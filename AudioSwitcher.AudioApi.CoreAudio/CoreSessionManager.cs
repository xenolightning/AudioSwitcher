using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Session;
using IAudioSessionManager = AudioSwitcher.AudioApi.Session.IAudioSessionManager;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CoreSessionManager : IAudioSessionManager
    {
        private IMMDevice _device;

        public CoreSessionManager(IMMDevice device)
        {
            ComThread.Assert();
            _device = device;
        }

        public event AudioSessionsChangedEventHandler AudioSessionsChanged;
        public IEnumerable<IAudioSession> All()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAudioSession>> AllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAudioSession> GetActiveSessions()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAudioSession>> GetActiveSessionsAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAudioSession> GetInactiveSessions()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAudioSession>> GetInactiveSessionsAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAudioSession> GetExpiredSessions()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAudioSession>> GetExpiredSessionsAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}