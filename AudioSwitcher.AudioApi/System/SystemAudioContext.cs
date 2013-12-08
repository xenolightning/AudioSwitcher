namespace AudioSwitcher.AudioApi.System
{
    public class SystemAudioContext : AudioContext
    {
        public SystemAudioContext()
            : this(null)
        {
        }

        public SystemAudioContext(IPreferredDeviceManager preferredDeviceManager)
            : base(new SystemAudioController(), preferredDeviceManager)
        {
        }
    }
}