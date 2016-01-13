
namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed partial class JavaScriptAudioDevice
    {
        public double Volume(double level)
        {
            Device.Volume = (int)level;
            return Device.Volume;
        }

        public double Volume()
        {
            return Device.Volume;
        }

        public bool Mute(bool mute)
        {
            Device.Mute(mute);
            return Device.IsMuted;
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
            Device.ToggleMute();
            return Device.IsMuted;
        }
    }
}