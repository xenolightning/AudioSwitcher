namespace AudioSwitcher.AudioApi.Tests.Stubs
{
    public abstract class DeviceStub : Device
    {
        public DeviceStub() : base(null)
        {
        }

        internal void Dispose()
        {
            base.Dispose(true);
        }

        public void FirePropertyChanged(string name)
        {
            OnPropertyChanged(name);
        }

        public void FireVolumeChanged(double volume)
        {
            OnVolumeChanged(volume);
        }

        public void FireMuteChanged(bool mute)
        {
            OnMuteChanged(mute);
        }

        public void FirePeakChanged(double volume)
        {
            OnPeakValueChanged(volume);
        }

        public void FireDefaultChanged()
        {
            OnDefaultChanged();
        }

        public void FireStateChanged(DeviceState state)
        {
            OnStateChanged(state);
        }
    }
}
