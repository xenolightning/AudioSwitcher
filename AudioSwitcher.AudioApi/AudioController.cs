using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class AudioController
    {
        private DeviceState _defaultDeviceStateFilter = DeviceState.Active | DeviceState.Unplugged |
                                                        DeviceState.Disabled;

        protected IDeviceEnumerator DeviceEnumerator { get; set; }

        /// <summary>
        ///     Set this to change the device state selector. Default is, Active | Unplugged | Disabled (everything except
        ///     NotPresent)
        /// </summary>
        public DeviceState DefaultDeviceStateFilter
        {
            get { return _defaultDeviceStateFilter; }
            set { _defaultDeviceStateFilter = value; }
        }

        public IEnumerable<AudioDevice> PlaybackDevices
        {
            get { return DeviceEnumerator.GetAudioDevices(DataFlow.Render, DefaultDeviceStateFilter); }
        }

        public IEnumerable<AudioDevice> RecordingDevices
        {
            get { return DeviceEnumerator.GetAudioDevices(DataFlow.Capture, DefaultDeviceStateFilter); }
        }

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

        public virtual AudioDevice GetAudioDevice(Guid id)
        {
            return GetAudioDevice(id, DefaultDeviceStateFilter);
        }

        public virtual AudioDevice GetAudioDevice(Guid id, DeviceState state)
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

    [ComVisible(false)]
    public abstract class AudioController<T> : AudioController
        where T : AudioDevice
    {
        protected AudioController(IDeviceEnumerator<T> devEnum)
        {
            DeviceEnumerator = devEnum;
            DeviceEnumerator.AudioDeviceChanged += DeviceEnumerator_AudioDeviceChanged;
        }

        protected new IDeviceEnumerator<T> DeviceEnumerator
        {
            get { return base.DeviceEnumerator as IDeviceEnumerator<T>; }
            set { base.DeviceEnumerator = value; }
        }

        public new IEnumerable<T> PlaybackDevices
        {
            get { return base.PlaybackDevices.Cast<T>(); }
        }

        public new IEnumerable<T> RecordingDevices
        {
            get { return base.RecordingDevices.Cast<T>(); }
        }

        public new T DefaultPlaybackDevice
        {
            get { return base.DefaultPlaybackDevice as T; }
        }

        public new T DefaultPlaybackCommDevice
        {
            get { return base.DefaultPlaybackCommDevice as T; }
        }

        public new T DefaultRecordingDevice
        {
            get { return base.DefaultRecordingDevice as T; }
        }

        public new T DefaultRecordingCommDevice
        {
            get { return base.DefaultRecordingCommDevice as T; }
        }

        private void DeviceEnumerator_AudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            OnAudioDeviceChanged(sender, e);
        }

        public new T GetAudioDevice(Guid id)
        {
            return GetAudioDevice(id, DefaultDeviceStateFilter);
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