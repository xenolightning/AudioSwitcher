using System;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed partial class JavaScriptAudioDevice
    {
        [JSFunction(Name = "volume")]
        public int Volume(int level)
        {
            Device.Volume = level;
            return Device.Volume;
        }

        [JSFunction(Name = "volume")]
        public int Volume()
        {
            return Device.Volume;
        }

        [JSFunction(Name = "mute")]
        public bool Mute(bool mute)
        {
            Device.Mute(mute);
            return Device.IsMuted;
        }

        [JSFunction(Name = "setAsDefault")]
        public bool SetAsDefault()
        {
            return Device.SetAsDefault();
        }

        [JSFunction(Name = "setAsDefaultComm")]
        public bool SetAsDefaultComm()
        {
            Device.SetAsDefaultCommunications();
            return true;
        }

        [JSFunction(Name = "toggleMute")]
        public bool ToggleMute()
        {
            Device.ToggleMute();
            return Device.IsMuted;
        }
    }
}