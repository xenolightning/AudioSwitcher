namespace AudioSwitcher.AudioApi.System
{
    public class SystemAudioContext : AudioContext
    {
        public SystemAudioContext()
            : this(null)
        {
        }

        public SystemAudioContext(PreferredDeviceManager preferredDeviceManager)
            : base(new SystemAudioController(), preferredDeviceManager)
        {
        }
    }
}