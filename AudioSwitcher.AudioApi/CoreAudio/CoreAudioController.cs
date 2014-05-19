using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [ComVisible(false)]
    public sealed class CoreAudioController : Controller<CoreAudioDevice>
    {
        public CoreAudioController()
            : base(new CoreAudioDeviceEnumerator())
        {
        }
    }
}