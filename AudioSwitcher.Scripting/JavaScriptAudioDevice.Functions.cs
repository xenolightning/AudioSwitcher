using Jurassic.Library;

namespace AudioSwitcher.Scripting
{
    public sealed partial class JavaScriptAudioDevice
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
            if (mute)
                Device.Mute();
            else
                Device.UnMute();

            return Device.IsMuted;
        }

        [JSFunction(Name = "setAsDefaultDevice")]
        public bool SetAsDefaultDevice()
        {
            Device.SetAsDefault();
            return true;
        }

        [JSFunction(Name = "setAsDefaultCommDevice")]
        public bool SetAsDefaultCommDevice()
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