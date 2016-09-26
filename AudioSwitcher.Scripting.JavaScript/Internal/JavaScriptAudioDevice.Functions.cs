
namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed partial class JavaScriptAudioDevice
    {
        public double Volume(double level)
        {
            return Device.SetVolumeAsync(level).Result;
        }

        public double Volume()
        {
            return Device.Volume;
        }

        public bool Mute(bool mute)
        {
            return Device.SetMuteAsync(mute).Result;
        }

        public bool SetAsDefault()
        {
            return Device.SetAsDefault();
        }

        public bool SetAsDefaultComm()
        {
            Device.SetAsDefaultCommunications();
            return true;
        }

        public bool ToggleMute()
        {
            return Device.ToggleMuteAsync().Result;
        }
    }
}