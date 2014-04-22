namespace AudioSwitcher.AudioApi.Isolated
{
    public class IsolatedAudioContext : AudioContext
    {
        public IsolatedAudioContext()
            : this(null)
        {
        }

        public IsolatedAudioContext(IPreferredDeviceManager preferredDeviceManager)
            : base(new IsolatedAudioController(), preferredDeviceManager)
        {
        }
    }
}