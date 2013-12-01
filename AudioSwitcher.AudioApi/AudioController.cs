using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class AudioController
    {
        protected const DeviceState DefaultDeviceStateFilter = DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled;

        protected IDeviceEnumerator DeviceEnumerator { get; set; }

        public virtual AudioDevice DefaultPlaybackDevice
        {
            get { return DeviceEnumerator.DefaultPlaybackDevice; }
        }

        public virtual AudioDevice DefaultPlaybackCommDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsPlaybackDevice; }
        }

        public virtual AudioDevice DefaultRecordingDevice
        {
            get { return DeviceEnumerator.DefaultRecordingDevice; }
        }

        public virtual AudioDevice DefaultRecordingCommDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsRecordingDevice; }
        }

        public virtual event AudioDeviceChangedHandler AudioDeviceChanged;

        protected virtual void OnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            //Bubble the event
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(sender, e);
        }

        public IEnumerable<AudioDevice> GetPlaybackDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetAudioDevices(DataFlow.Render, deviceState);
        }

        public IEnumerable<AudioDevice> GetRecordingDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetAudioDevices(DataFlow.Capture, deviceState);
        }

        public virtual AudioDevice GetAudioDevice(Guid id, DeviceState state = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetAudioDevices(DataFlow.All, state).FirstOrDefault(dev => dev.ID == id);
        }

        public virtual bool SetDefaultDevice(AudioDevice dev)
        {
            return DeviceEnumerator.SetDefaultDevice(dev);
        }

        public virtual bool SetDefaultCommunicationsDevice(AudioDevice dev)
        {
            return DeviceEnumerator.SetDefaultCommunicationsDevice(dev);
        }
    }
}