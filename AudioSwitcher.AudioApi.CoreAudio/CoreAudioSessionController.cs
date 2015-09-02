using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CoreAudioSessionController : IAudioSessionController, IAudioSessionNotification
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly IAudioSessionManager2 _audioSessionManager2;

        private List<CoreAudioSession> _sessionCache;

        public CoreAudioSessionController(IAudioSessionManager2 audioSessionManager2)
        {
            if (audioSessionManager2 == null)
                throw new ArgumentNullException("audioSessionManager2");

            ComThread.Assert();

            _audioSessionManager2 = audioSessionManager2;
            _audioSessionManager2.RegisterSessionNotification(this);

            RefreshSessions();
        }

        private void RefreshSessions()
        {
            IAudioSessionEnumerator enumerator;
            _audioSessionManager2.GetSessionEnumerator(out enumerator);

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

                    _sessionCache.Add(new CoreAudioSession(session));
                }
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
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
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

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
                    _lock.ExitReadLock();
            }

            return 0;
        }
    }
}