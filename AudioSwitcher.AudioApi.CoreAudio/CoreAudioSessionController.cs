using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using System.Management;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class CoreAudioSessionController : IAudioSessionController, IAudioSessionNotification
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly IAudioSessionManager2 _audioSessionManager;

        private List<CoreAudioSession> _sessionCache;

        private readonly object _sessionObserverLock = new object();
        private readonly List<IObserver<AudioSessionsChanged>> _sessionObservers;
        private readonly ManagementEventWatcher _processStopEvent;

        public CoreAudioSessionController(IAudioSessionManager2 audioSessionManager)
        {
            if (audioSessionManager == null)
                throw new ArgumentNullException("audioSessionManager");

            ComThread.Assert();

            _audioSessionManager = audioSessionManager;
            _audioSessionManager.RegisterSessionNotification(this);

            _sessionObservers = new List<IObserver<AudioSessionsChanged>>();

            RefreshSessions();

            _processStopEvent = new ManagementEventWatcher();

            //Subscribe to process exit events from WMI
            _processStopEvent.Query = new WqlEventQuery("__InstanceDeletionEvent", new TimeSpan(0, 0, 1), "TargetInstance isa \"Win32_Process\"");
            _processStopEvent.EventArrived += (sender, args) =>
            {
                var process = args.NewEvent["TargetInstance"] as ManagementBaseObject;

                if (process == null)
                    return;

                var processId = (int) (uint) process["ProcessId"]; //This is silly but WMI won't cast directly to an int

                RemoveSessions(_sessionCache.Where(x => x.ProcessId == processId));
            };

            _processStopEvent.Start();
        }

        public void Dispose()
        {
            _processStopEvent.Stop();
            _processStopEvent.Dispose();

            lock (_sessionObserverLock)
            {
                _sessionObservers.ForEach(x => x.OnCompleted());
                _sessionObservers.Clear();
            }

            Marshal.FinalReleaseComObject(_audioSessionManager);
        }

        public IEnumerable<IAudioSession> All()
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return _sessionCache.ToList();
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        public Task<IEnumerable<IAudioSession>> AllAsync()
        {
            return Task.Factory.StartNew(() => All());
        }

        public IEnumerable<IAudioSession> GetActiveSessions()
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return _sessionCache.Where(x => x.SessionState == AudioSessionState.Active).ToList();
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        public Task<IEnumerable<IAudioSession>> GetActiveSessionsAsync()
        {
            return Task.Factory.StartNew(() => GetActiveSessions());
        }

        public IEnumerable<IAudioSession> GetInactiveSessions()
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return _sessionCache.Where(x => x.SessionState == AudioSessionState.Inactive).ToList();
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        public Task<IEnumerable<IAudioSession>> GetInactiveSessionsAsync()
        {
            return Task.Factory.StartNew(() => GetInactiveSessions());
        }

        public IEnumerable<IAudioSession> GetExpiredSessions()
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return _sessionCache.Where(x => x.SessionState == AudioSessionState.Expired).ToList();
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        public Task<IEnumerable<IAudioSession>> GetExpiredSessionsAsync()
        {
            return Task.Factory.StartNew(() => GetExpiredSessions());
        }

        public int OnSessionCreated(IAudioSessionControl sessionControl)
        {
            var acquiredLock = _lock.AcquireWriteLockNonReEntrant();


            try
            {
                var ptr = Marshal.GetIUnknownForObject(sessionControl);
                Marshal.AddRef(ptr);
                var managedSession = ComThread.Invoke(() => CreateSessionWrapper(sessionControl));

                lock (_sessionObserverLock)
                {
                    _sessionObservers.ForEach(x => x.OnNext(new AudioSessionsChanged(managedSession.Id, AudioSessionsChangedType.Created)));
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitWriteLock();
            }

            return 0;
        }

        public IDisposable Subscribe(IObserver<AudioSessionsChanged> observer)
        {
            lock (_sessionObservers)
            {
                _sessionObservers.Add(observer);
            }

            return DelegateDisposable.Create(() =>
            {
                lock (_sessionObserverLock)
                {
                    _sessionObservers.Remove(observer);
                }
            });
        }

        private void RefreshSessions()
        {
            IAudioSessionEnumerator enumerator;
            _audioSessionManager.GetSessionEnumerator(out enumerator);

            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                int count;
                enumerator.GetCount(out count);

                _sessionCache = new List<CoreAudioSession>(count);

                for (var i = 0; i < count; i++)
                {
                    IAudioSessionControl session;
                    enumerator.GetSession(i, out session);
                    CreateSessionWrapper(session);
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        private CoreAudioSession CreateSessionWrapper(IAudioSessionControl session)
        {
            var managedSession = new CoreAudioSession(session);

            managedSession.Subscribe<AudioSessionStateChanged>(ManagedSessionOnStateChanged);
            managedSession.Subscribe<AudioSessionDisconnected>(ManagedSessionOnDisconnected);

            _sessionCache.Add(managedSession);

            return managedSession;
        }

        private void ManagedSessionOnStateChanged(AudioSessionStateChanged changed)
        {
            //Asynchronously check that the process is still alive after a session change
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
            })
            .ContinueWith(task =>
            {
            });
        }

        private void ManagedSessionOnDisconnected(AudioSessionDisconnected disconnected)
        {
            var sessions = _sessionCache.Where(x => x.Id == disconnected.Session.Id);

            RemoveSessions(sessions);

            lock (_sessionObserverLock)
            {
                _sessionObservers.ForEach(x => x.OnNext(new AudioSessionsChanged(disconnected.Session.Id, AudioSessionsChangedType.Disconnected)));
            }
        }

        private void RemoveSessions(IEnumerable<CoreAudioSession> sessions)
        {
            var coreAudioSessions = sessions as IList<CoreAudioSession> ?? sessions.ToList();

            if (!coreAudioSessions.Any())
                return;

            var acquiredLock = _lock.AcquireWriteLockNonReEntrant();

            try
            {
                _sessionCache.RemoveAll(x => coreAudioSessions.Any(y => y.Id == x.Id));

                foreach (var s in coreAudioSessions)
                {
                    lock (_sessionObserverLock)
                    {
                        _sessionObservers.ForEach(x => x.OnNext(new AudioSessionsChanged(s.Id, AudioSessionsChangedType.Disconnected)));
                    }

                    s.Dispose();
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitWriteLock();
            }
        }

    }
}