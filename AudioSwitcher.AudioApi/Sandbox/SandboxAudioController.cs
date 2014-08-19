namespace AudioSwitcher.AudioApi.Sandbox
{
    public class SandboxAudioController : AudioController<SandboxDevice>
    {
        public SandboxAudioController(IDeviceEnumerator enumerator)
            : base(new SandboxDeviceEnumerator(enumerator))
        {
            DeviceEnumerator.AudioDeviceChanged += DeviceEnumerator_AudioDeviceChanged;
        }

        private void DeviceEnumerator_AudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
        }
    }
}