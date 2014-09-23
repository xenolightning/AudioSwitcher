using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class AudioController
    {
        protected const DeviceState DefaultDeviceStateFilter =
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
        }

        public virtual IDevice DefaultPlaybackCommunicationsDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsPlaybackDevice; }
        }

        public virtual IDevice DefaultCaptureDevice
        {
            get { return DeviceEnumerator.DefaultCaptureDevice; }
        }

        public virtual IDevice DefaultCaptureCommunicationsDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsCaptureDevice; }
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        private void DeviceEnumerator_AudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            //Bubble the event
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(sender, e);

            OnAudioDeviceChanged(sender, e);
        }

        protected virtual void OnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
        }

        public IEnumerable<IDevice> GetAllDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DeviceType.All, deviceState);
        }

        public Task<IEnumerable<IDevice>> GetAllDevicesAsync(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.All, deviceState);
        }

        public IEnumerable<IDevice> GetPlaybackDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Playback, deviceState);
        }

        public Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Playback, deviceState);
        }

        public IEnumerable<IDevice> GetCaptureDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Capture, deviceState);
        }

        public Task<IEnumerable<IDevice>> GetCaptureDevicesAsync(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Capture, deviceState);
        }

        public virtual IDevice GetAudioDevice(Guid id, DeviceState state = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DeviceType.All, state).FirstOrDefault(dev => dev.Id == id);
        }

        public virtual Task<IDevice> GetAudioDeviceAsync(Guid id, DeviceState state = DefaultDeviceStateFilter)
        {
            return Task.Factory.StartNew(() => DeviceEnumerator.GetDevices(DeviceType.All, state).FirstOrDefault(dev => dev.Id == id));
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