using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class AudioController<T> : AudioController
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
        }

        public T DefaultPlaybackCommDevice
        {
            get { return base.DefaultPlaybackCommunicationsDevice as T; }
        }

        public new T DefaultCaptureDevice
        {
            get { return base.DefaultCaptureDevice as T; }
        }

        public T DefaultCaptureCommDevice
        {
            get { return base.DefaultCaptureCommunicationsDevice as T; }
        }

        public new IEnumerable<T> GetPlaybackDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Playback, deviceState);
        }
        public new Task<IEnumerable<T>> GetPlaybackDevicesAsync(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Playback, deviceState);
        }

        public new IEnumerable<T> GetCaptureDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DeviceType.Capture, deviceState);
        }

        public new Task<IEnumerable<T>> GetCaptureDevicesAsync(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevicesAsync(DeviceType.Capture, deviceState);
        }

        public new T GetAudioDevice(Guid id, DeviceState state = DefaultDeviceStateFilter)
        {
            return base.GetAudioDevice(id, state) as T;
        }

        public new Task<T> GetAudioDeviceAsync(Guid id, DeviceState state = DefaultDeviceStateFilter)
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