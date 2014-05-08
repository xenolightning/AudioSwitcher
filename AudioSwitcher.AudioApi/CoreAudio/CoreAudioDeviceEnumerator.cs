using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     Enumerates Windwows System Devices.
    ///     Stores the current devices in memory to avoid calling the COM library when not requried
    /// </summary>
    [ComVisible(false)]
    public sealed class CoreAudioDeviceEnumerator : IDeviceEnumerator<CoreAudioDevice>, IMMNotificationClient,
        IDisposable
    {
        private readonly object _mutex = new object();
        internal MMDeviceEnumerator InnerEnumerator;
        private ConcurrentBag<CoreAudioDevice> _deviceCache = new ConcurrentBag<CoreAudioDevice>();

        public CoreAudioDeviceEnumerator()
        {
            InnerEnumerator = new MMDeviceEnumerator();
            InnerEnumerator.RegisterEndpointNotificationCallback(this);
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        public AudioController Controller { get; set; }

        public CoreAudioDevice DefaultPlaybackDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Render, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsPlaybackDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Render, Role.Communications); }
        }

        public CoreAudioDevice DefaultRecordingDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Capture, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsRecordingDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Capture, Role.Communications); }
        }

        AudioDevice IDeviceEnumerator.DefaultPlaybackDevice
        {
            get { return DefaultPlaybackDevice; }
        }

        AudioDevice IDeviceEnumerator.DefaultCommunicationsPlaybackDevice
        {
            get { return DefaultCommunicationsPlaybackDevice; }
        }

        AudioDevice IDeviceEnumerator.DefaultRecordingDevice
        {
            get { return DefaultRecordingDevice; }
        }

        AudioDevice IDeviceEnumerator.DefaultCommunicationsRecordingDevice
        {
            get { return DefaultCommunicationsRecordingDevice; }
        }

        AudioDevice IDeviceEnumerator.GetAudioDevice(Guid id)
        {
            return GetAudioDevice(id);
        }

        AudioDevice IDeviceEnumerator.GetDefaultAudioDevice(DataFlow dataflow, Role eRole)
        {
            return GetDefaultAudioDevice(dataflow, eRole);
        }

        IEnumerable<AudioDevice> IDeviceEnumerator.GetAudioDevices(DataFlow dataflow, DeviceState eRole)
        {
            return GetAudioDevices(dataflow, eRole);
        }

        public void Dispose()
        {
            if (InnerEnumerator != null)
                InnerEnumerator.UnregisterEndpointNotificationCallback(this);

            _deviceCache = null;
            InnerEnumerator = null;
        }

        private void RefreshSystemDevices()
        {
            lock (_mutex)
            {
                _deviceCache = new ConcurrentBag<CoreAudioDevice>();
                foreach (MMDevice mDev in InnerEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
                {
                    _deviceCache.Add(new CoreAudioDevice(mDev, this));
                }
            }
        }

        public void OnAudioDeviceChanged(AudioDeviceChangedEventArgs e)
        {
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(e.Device, e);
        }

        #region IDevEnum Members

        public CoreAudioDevice GetAudioDevice(Guid id)
        {
            lock (_mutex)
            {
                RefreshSystemDevices();
                return _deviceCache.FirstOrDefault(x => x.Id == id);
            }
        }

        public bool SetDefaultDevice(AudioDevice dev)
        {
            return SetDefaultDevice(dev as CoreAudioDevice);
        }

        public bool SetDefaultCommunicationsDevice(AudioDevice dev)
        {
            return SetDefaultCommunicationsDevice(dev as CoreAudioDevice);
        }

        public bool SetDefaultDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            try
            {
                if (Environment.OSVersion.Version.Major > 6
                    || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1)
                    )
                    PolicyConfig.SetDefaultEndpoint(dev.Device.ID, Role.Console | Role.Multimedia);
                else
                    PolicyConfigVista.SetDefaultEndpoint(dev.Device.ID, Role.Console | Role.Multimedia);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetDefaultCommunicationsDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            try
            {
                if (Environment.OSVersion.Version.Major > 6
                    || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1)
                    )
                    PolicyConfig.SetDefaultEndpoint(dev.Device.ID, Role.Communications);
                else
                    PolicyConfigVista.SetDefaultEndpoint(dev.Device.ID, Role.Communications);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public CoreAudioDevice GetDefaultAudioDevice(DataFlow dataflow, Role eRole)
        {
            lock (_mutex)
            {
                RefreshSystemDevices();

                MMDevice defDev = InnerEnumerator.GetDefaultAudioEndpoint(dataflow, eRole);
                if (defDev == null || string.IsNullOrEmpty(defDev.ID))
                    return null;

                return _deviceCache.First(x => x.Device.ID == defDev.ID);
            }
        }

        public IEnumerable<CoreAudioDevice> GetAudioDevices(DataFlow dataflow, DeviceState state)
        {
            return
                InnerEnumerator.EnumerateAudioEndPoints(dataflow, state)
                    .Select((x, y) => new CoreAudioDevice(x, this));
        }

        #endregion

        #region IMMNotif Members

        private Tuple<string, DataFlow, Role, DateTime> _lastEvent;

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.StateChanged));
        }

        void IMMNotificationClient.OnDeviceAdded(string deviceId)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Added));
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Removed));
        }

        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string deviceId)
        {
            //Need to do some event filtering here, there's a scenario where
            //multiple default device changed are raised when one playback device changes.
            //This is correct functionality, but I want to limit it to one event per device
            //Console === Multimedia device for my purpose
            //Assume any events that happen within 200ms are the same
            if (_lastEvent != null
                && _lastEvent.Item1 == deviceId
                && _lastEvent.Item2 == flow
                && (role == Role.Console || role == Role.Multimedia)
                && DateTime.Now.Subtract(_lastEvent.Item4).Milliseconds < 200)
            {
                return;
            }

            _lastEvent = new Tuple<string, DataFlow, Role, DateTime>(deviceId, flow, role, DateTime.Now);

            if (role == Role.Console || role == Role.Multimedia)
                OnAudioDeviceChanged(
                    new AudioDeviceChangedEventArgs(GetAudioDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                        AudioDeviceEventType.DefaultDevice));
            else if (role == Role.Communications)
                OnAudioDeviceChanged(
                    new AudioDeviceChangedEventArgs(GetAudioDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                        AudioDeviceEventType.DefaultCommunicationsDevice));
        }

        void IMMNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

        #endregion
    }
}