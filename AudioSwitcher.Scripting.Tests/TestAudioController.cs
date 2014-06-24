using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Scripting.Tests
{
    public sealed class TestAudioController : AudioController
    {
        public TestAudioController(IDeviceEnumerator enumerator)
            : base(enumerator)
        {
        }
    }
}
