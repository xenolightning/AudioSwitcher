using AudioSwitcher.AudioApi;

namespace AudioSwitcher.AudioApi.Isolated
{
    public class IsolatedAudioController : AudioController<IsolatedAudioDevice>
    {
        public IsolatedAudioController()
            : base(new DebugSystemDeviceEnumerator())
        {
        }
    }
}
