using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioController
    {
        protected const DeviceState DEFAULT_DEVICE_STATE_FILTER =
            DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled;

        protected AudioController(IDeviceEnumerator enumerator)
        {
            DeviceEnumerator = enumerator;
            DeviceEnumerator.AudioDeviceChanged += DeviceEnumerator_AudioDeviceChanged;

            enumerator.AudioController = this;
        }

        protected IDeviceEnumerator DeviceEnumerator { get; set; }

        public virtual IDevice DefaultPlaybackDevice
        {
            get { return DeviceEnumerator.DefaultPlaybackDevice; }
            set { DeviceEnumerator.SetDefaultDevice(value); }
        }

        public virtual IDevice DefaultPlaybackCommunicationsDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsPlaybackDevice; }
            set { DeviceEnumerator.SetDefaultCommunicationsDevice(value); }
        }

        public virtual IDevice DefaultCaptureDevice
        {
            get { return DeviceEnumerator.DefaultCaptureDevice; }
            set { DeviceEnumerator.SetDefaultDevice(value); }
        }

        public virtual IDevice DefaultCaptureCommunicationsDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsCaptureDevice; }
            set { DeviceEnumerator.SetDefaultCommunicationsDevice(value); }
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        private void DeviceEnumerator_AudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            OnAudioDeviceChanged(sender, e);
        }

        protected virtual void OnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            var handler = AudioDeviceChanged;

            //Bubble the event
            if (handler != null)
                handler(sender, e);
        }

        public IEnumerable<IDevice> GetAllDevices(DeviceState deviceState = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevices(DeviceType.All, deviceState);
        }

        public Task<IEnumerable<IDevice>> GetAllDevicesAsync(DeviceState deviceState = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.All, deviceState);
        }

        public IEnumerable<IDevice> GetPlaybackDevices(DeviceState deviceState = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Playback, deviceState);
        }

        public Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync(DeviceState deviceState = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Playback, deviceState);
        }

        public IEnumerable<IDevice> GetCaptureDevices(DeviceState deviceState = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Capture, deviceState);
        }

        public Task<IEnumerable<IDevice>> GetCaptureDevicesAsync(DeviceState deviceState = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Capture, deviceState);
        }

        public virtual IDevice GetAudioDevice(Guid id, DeviceState state = DEFAULT_DEVICE_STATE_FILTER)
        {
            return DeviceEnumerator.GetDevice(id, state);
        }

        public virtual Task<IDevice> GetAudioDeviceAsync(Guid id, DeviceState state = DEFAULT_DEVICE_STATE_FILTER)
        {
            return Task.Factory.StartNew(() => GetAudioDevice(id, state));
        }

        public virtual bool SetDefaultDevice(IDevice dev)
        {
            return DeviceEnumerator.SetDefaultDevice(dev);
        }

        public virtual bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            return DeviceEnumerator.SetDefaultCommunicationsDevice(dev);
        }

        public virtual Task<bool> SetDefaultDeviceAsync(IDevice dev)
        {
            return DeviceEnumerator.SetDefaultDeviceAsync(dev);
        }

        public virtual Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev)
        {
            return DeviceEnumerator.SetDefaultCommunicationsDeviceAsync(dev);
        }

    }
}