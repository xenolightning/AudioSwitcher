namespace AudioSwitcher.AudioApi.CoreAudio
{
    public class CoreAudioContext : AudioContext
    {
        public CoreAudioContext()
            : this(null)
        {
        }

        public CoreAudioContext(IPreferredDeviceManager preferredDeviceManager)
            : base(new CoreAudioController(), preferredDeviceManager)
        {
        }
    }
}