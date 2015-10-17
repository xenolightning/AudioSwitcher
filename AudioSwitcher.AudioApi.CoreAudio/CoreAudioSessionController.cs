using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class CoreAudioSessionController : IAudioSessionController, IAudioSessionNotification
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly IAudioSessionManager2 _audioSessionManager;

        private List<CoreAudioSession> _sessionCache;

        private readonly ManagementEventWatcher _processStopEvent;
        private readonly AsyncBroadcaster<AudioSessionChanged> _sessionChanged;

        public CoreAudioSessionController(IAudioSessionManager2 audioSessionManager)
        {
            if (audioSessionManager == null)
                throw new ArgumentNullException("audioSessionManager");

            ComThread.Assert();

            _audioSessionManager = audioSessionManager;
            _audioSessionManager.RegisterSessionNotification(this);

            _sessionChanged = new AsyncBroadcaster<AudioSessionChanged>();

            RefreshSessions();

            _processStopEvent = new ManagementEventWatcher();

            //Subscribe to process exit events from WMI
            _processStopEvent.Query = new WqlEventQuery("__InstanceDeletionEvent", new TimeSpan(0, 0, 1), "TargetInstance isa \"Win32_Process\"");
            _processStopEvent.EventArrived += (sender, args) =>
            {
                var process = args.NewEvent["TargetInstance"] as ManagementBaseObject;

                if (process == null)
                    return;

                var processId = Convert.ToInt32(process["ProcessId"]);

                RemoveSessions(_sessionCache.Where(x => x.ProcessId == processId));
            };

            _processStopEvent.Start();
        }

        public void Dispose()
        {
            _processStopEvent.Stop();
            _processStopEvent.Dispose();

            _sessionChanged.Dispose();

            Marshal.FinalReleaseComObject(_audioSessionManager);
        }

        public IObservable<AudioSessionChanged> SessionChanged
        {
            get
            {
                return _sessionChanged.AsObservable();
            }
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
            var ptr = Marshal.GetIUnknownForObject(sessionControl);
            Marshal.AddRef(ptr);
            var managedSession = ComThread.Invoke(() => CacheSessionWrapper(sessionControl));

            FireSessionChanged(managedSession, AudioSessionChangedType.Created);

            return 0;
        }

        private void RefreshSessions()
        {
            IAudioSessionEnumerator enumerator;
            _audioSessionManager.GetSessionEnumerator(out enumerator);
            int count;
            enumerator.GetCount(out count);

            _sessionCache = new List<CoreAudioSession>(count);

            for (var i = 0; i < count; i++)
            {
                IAudioSessionControl session;
                enumerator.GetSession(i, out session);
                var managedSession = CacheSessionWrapper(session);
                FireSessionChanged(managedSession, AudioSessionChangedType.Created);
            }
        }

        private CoreAudioSession CacheSessionWrapper(IAudioSessionControl session)
        {
            var managedSession = new CoreAudioSession(session);

            var acquiredLock = _lock.AcquireReadLockNonReEntrant();
            try
            {
                var existing = _sessionCache.FirstOrDefault(x => x.ProcessId == managedSession.ProcessId && String.Equals(x.Id, managedSession.Id));
                if (existing != null)
                {
                    managedSession.Dispose();
                    return existing;
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }

            managedSession.StateChanged.Subscribe(ManagedSessionOnStateChanged);
            managedSession.Disconnected.Subscribe(ManagedSessionOnDisconnected);

            acquiredLock = _lock.AcquireWriteLockNonReEntrant();

            try
            {
                _sessionCache.Add(managedSession);
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitWriteLock();
            }

            return managedSession;
        }

        private void ManagedSessionOnStateChanged(SessionStateChangedArgs changedArgs)
        {
        }

        private void ManagedSessionOnDisconnected(SessionDisconnectedArgs disconnectedArgs)
        {
            var sessions = _sessionCache.Where(x => x.Id == disconnectedArgs.Session.Id);

            RemoveSessions(sessions);
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
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitWriteLock();
            }

            foreach (var s in coreAudioSessions)
            {
                FireSessionChanged(s, AudioSessionChangedType.Disconnected);
                s.Dispose();
            }
        }

        private void FireSessionChanged(IAudioSession session, AudioSessionChangedType type)
        {
            _sessionChanged.OnNext(new AudioSessionChanged(session.Id, type));
        }

        public IEnumerator<IAudioSession> GetEnumerator()
        {
            return All().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}