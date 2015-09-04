using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Session;
using Timer = System.Timers.Timer;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CoreAudioSessionController : IAudioSessionController, IAudioSessionNotification
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly IAudioSessionManager2 _audioSessionManager;

        private List<CoreAudioSession> _sessionCache;
        private Timer _processTimer;

        public CoreAudioSessionController(IAudioSessionManager2 audioSessionManager)
        {
            if (audioSessionManager == null)
                throw new ArgumentNullException("audioSessionManager");

            ComThread.Assert();

            _audioSessionManager = audioSessionManager;
            _audioSessionManager.RegisterSessionNotification(this);

            RefreshSessions();

            _processTimer = new Timer
            {
                Interval = 2000,
                AutoReset = true,
                Enabled = true,
            };

            _processTimer.Elapsed += (sender, args) =>
            {
                _processTimer.Enabled = false;

                var processes = Process.GetProcesses();

                //remove all sessions from processes that don't exist
                RemoveSessions(_sessionCache.Where(x => processes.All(y => y.Id != x.ProcessId)));

                _processTimer.Enabled = true;
            };
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
                    var managedSession = new CoreAudioSession(session);
                    //managedSession.Disconnected += ManagedSessionOnDisconnected;
                    //managedSession.StateChanged += ManagedSessionOnStateChanged;

                    _sessionCache.Add(managedSession);
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        private void ManagedSessionOnStateChanged(IAudioSession sender, AudioSessionState state)
        {
            FireSessionChanged();
        }

        private void ManagedSessionOnDisconnected(IAudioSession sender)
        {
            var sessions = _sessionCache.Where(x => x.SessionId == sender.SessionId);

            RemoveSessions(sessions);

            FireSessionChanged();
        }

        private void FireSessionChanged()
        {
            var handler = SessionChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void RemoveSessions(IEnumerable<CoreAudioSession> sessions)
        {
            var coreAudioSessions = sessions as IList<CoreAudioSession> ?? sessions.ToList();

            if (!coreAudioSessions.Any())
                return;

            var acquiredLock = _lock.AcquireWriteLockNonReEntrant();

            try
            {
                _sessionCache.RemoveAll(x => coreAudioSessions.Any(y => y.SessionId == x.SessionId));

                foreach (var s in coreAudioSessions)
                {
                    //s.StateChanged -= ManagedSessionOnStateChanged;
                    //s.Disconnected -= ManagedSessionOnDisconnected;

                    s.Dispose();
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitWriteLock();
            }
        }

        public event EventHandler SessionChanged;

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
                ComThread.Invoke(() =>
                {
                    _sessionCache.Add(new CoreAudioSession(sessionControl));
                });
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitWriteLock();
            }


            FireSessionChanged();

            return 0;
        }

        public void Dispose()
        {
            Marshal.FinalReleaseComObject(_audioSessionManager);
        }

    }
}