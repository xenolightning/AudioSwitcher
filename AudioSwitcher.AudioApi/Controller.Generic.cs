using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class Controller<T> : Controller
        where T : Device
    {
        protected Controller(IDeviceEnumerator<T> devEnum)
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

        public new T DefaultRecordingDevice
        {
            get { return base.DefaultRecordingDevice as T; }
        }

        public T DefaultRecordingCommDevice
        {
            get { return base.DefaultRecordingCommunicationsDevice as T; }
        }

        public new IEnumerable<T> GetPlaybackDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DataFlow.Render, deviceState);
        }

        public new IEnumerable<T> GetRecordingDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DataFlow.Capture, deviceState);
        }

        public new T GetAudioDevice(Guid id, DeviceState state)
        {
            return base.GetAudioDevice(id, state) as T;
        }

        public virtual bool SetDefaultDevice(T dev)
        {
            return base.SetDefaultDevice(dev);
        }

        public virtual bool SetDefaultCommunicationsDevice(T dev)
        {
            return base.SetDefaultCommunicationsDevice(dev);
        }
    }
}