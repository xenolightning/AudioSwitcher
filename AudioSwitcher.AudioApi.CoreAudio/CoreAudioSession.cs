using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CoreAudioSession : IAudioSession
    {
        private readonly IAudioSessionControl2 _control;
        private readonly ISimpleAudioVolume _volume;
        public string SessionId { get; private set; }
        public int ProcessId { get; private set; }
        public int ProcessName { get; private set; }
        public bool IsSystemSession { get; private set; }
        public int Volume { get; set; }
        public AudioSessionState SessionState { get; private set; }

        public CoreAudioSession(IAudioSessionControl control)
        {
            _control = control as IAudioSessionControl2;
            _volume = control as ISimpleAudioVolume;
        }
    }
}
