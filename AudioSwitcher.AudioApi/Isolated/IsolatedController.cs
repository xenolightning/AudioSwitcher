namespace AudioSwitcher.AudioApi.Isolated
{
    public class IsolatedController : Controller<IsolatedDevice>
    {
        public IsolatedController()
            : base(new DebugSystemDeviceEnumerator())
        {
            this.DeviceEnumerator.AudioDeviceChanged += DeviceEnumerator_AudioDeviceChanged;
        }

        void DeviceEnumerator_AudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
        }
    }
}