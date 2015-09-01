using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CoreAudioSessionController : IAudioSessionController, IAudioSessionNotification
    {
        private readonly IAudioSessionManager2 _audioSessionManager2;

        private readonly List<CoreAudioSession> _sessionCache;

        public CoreAudioSessionController(IAudioSessionManager2 audioSessionManager2)
        {
            if (audioSessionManager2 == null)
                throw new ArgumentNullException("audioSessionManager2");

            _audioSessionManager2 = audioSessionManager2;
            _audioSessionManager2.RegisterSessionNotification(this);

            IAudioSessionEnumerator enumerator;
            _audioSessionManager2.GetSessionEnumerator(out enumerator);

            int count;
            enumerator.GetCount(out count);

            _sessionCache = new List<CoreAudioSession>(count);

            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl session;
                enumerator.GetSession(i, out session);
                var asdf = session as IAudioSessionControl2;
                var vol = session as ISimpleAudioVolume;
                string asdfas;
                asdf.GetSessionIdentifier(out asdfas);
                string fair;
                asdf.GetSessionInstanceIdentifier(out fair);

                float dsafdsafdsfdsafdsafdsafdsafdsa;
                vol.GetMasterVolume(out dsafdsafdsfdsafdsafdsafdsafdsa);
            }

        }

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

        public int OnSessionCreated(IAudioSessionControl sessionControl)
        {
            return -1;
        }
    }
}