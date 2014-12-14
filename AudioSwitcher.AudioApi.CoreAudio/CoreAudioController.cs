using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed class CoreAudioController : AudioController<CoreAudioDevice>
    {
        public CoreAudioController()
            : base(new CoreAudioDeviceEnumerator())
        {
        }
    }
}