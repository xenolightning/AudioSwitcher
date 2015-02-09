using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioController<T> : AudioController, IAudioController<T>
        where T : Device
    {
        protected AudioController(IDeviceEnumerator<T> devEnum)
            : base(devEnum)
        {
        }

        protected new IDeviceEnumerator<T> DeviceEnumerator
        {
            get { return base.DeviceEnumerator as IDeviceEnumerator<T>; }
            set { base.DeviceEnumerator = value; }
        }

        public new T DefaultPlaybackDevice
        {
            get { return base.DefaultPlaybackDevice as T; }
            set { DeviceEnumerator.SetDefaultDevice(value); }
        }

        public new T DefaultPlaybackCommunicationsDevice
        {
            get { return base.DefaultPlaybackCommunicationsDevice as T; }
            set { DeviceEnumerator.SetDefaultCommunicationsDevice(value); }
        }

        public new T DefaultCaptureDevice
        {
            get { return base.DefaultCaptureDevice as T; }
            set { DeviceEnumerator.SetDefaultDevice(value); }
        }

        public new T DefaultCaptureCommunicationsDevice
        {
            get { return base.DefaultCaptureCommunicationsDevice as T; }
            set { DeviceEnumerator.SetDefaultCommunicationsDevice(value); }
        }

        public new IEnumerable<T> GetAllDevices()
        {
            return GetAllDevices(DEFAULT_DEVICE_STATE_FILTER);
        }

        public new IEnumerable<T> GetAllDevices(DeviceState deviceState)
        {
            return DeviceEnumerator.GetDevices(DeviceType.All, deviceState);
        }

        public new Task<IEnumerable<T>> GetAllDevicesAsync()
        {
            return GetAllDevicesAsync(DEFAULT_DEVICE_STATE_FILTER);
        }

        public new Task<IEnumerable<T>> GetAllDevicesAsync(DeviceState deviceState)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.All, deviceState);
        }

        public new IEnumerable<T> GetPlaybackDevices()
        {
            return GetPlaybackDevices(DEFAULT_DEVICE_STATE_FILTER);
        }

        public new IEnumerable<T> GetPlaybackDevices(DeviceState deviceState)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Playback, deviceState);
        }

        public new Task<IEnumerable<T>> GetPlaybackDevicesAsync()
        {
            return GetPlaybackDevicesAsync(DEFAULT_DEVICE_STATE_FILTER);
        }

        public new Task<IEnumerable<T>> GetPlaybackDevicesAsync(DeviceState deviceState)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Playback, deviceState);
        }

        public new IEnumerable<T> GetCaptureDevices()
        {
            return GetCaptureDevices(DEFAULT_DEVICE_STATE_FILTER);
        }

        public new IEnumerable<T> GetCaptureDevices(DeviceState deviceState)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Capture, deviceState);
        }

        public new Task<IEnumerable<T>> GetCaptureDevicesAsync()
        {
            return GetCaptureDevicesAsync(DEFAULT_DEVICE_STATE_FILTER);
        }

        public new Task<IEnumerable<T>> GetCaptureDevicesAsync(DeviceState deviceState)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Capture, deviceState);
        }

        public new T GetAudioDevice(Guid id)
        {
            return GetAudioDevice(id, DeviceState.All);
        }

        public new T GetAudioDevice(Guid id, DeviceState state)
        {
            return base.GetAudioDevice(id, state) as T;
        }

        public new Task<T> GetAudioDeviceAsync(Guid id)
        {
            return GetAudioDeviceAsync(id, DeviceState.All);
        }

        public new Task<T> GetAudioDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => DeviceEnumerator.GetDevices(DeviceType.All, state).FirstOrDefault(dev => dev.Id == id));
        }

        public virtual bool SetDefaultDevice(T dev)
        {
            return base.SetDefaultDevice(dev);
        }

        public virtual Task<bool> SetDefaultDeviceAsync(T dev)
        {
            return base.SetDefaultDeviceAsync(dev);
        }

        public virtual bool SetDefaultCommunicationsDevice(T dev)
        {
            return base.SetDefaultCommunicationsDevice(dev);
        }

        public virtual Task<bool> SetDefaultCommunicationsDeviceAsync(T dev)
        {
            return base.SetDefaultCommunicationsDeviceAsync(dev);
        }
    }
}