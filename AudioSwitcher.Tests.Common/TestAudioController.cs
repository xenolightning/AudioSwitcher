using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public class TestAudioController : AudioController<TestDevice>
    {
        public TestAudioController(IDeviceEnumerator<TestDevice> devEnum)
            : base(devEnum)
        {
        }
    }
}
