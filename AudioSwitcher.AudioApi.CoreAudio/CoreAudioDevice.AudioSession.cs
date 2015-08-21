using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public partial class CoreAudioDevice : IAudioSessionEndpoint
    {
        public IAudioSessionManager SessionManager
        {
            get;
            private set;
        }
    }
}
