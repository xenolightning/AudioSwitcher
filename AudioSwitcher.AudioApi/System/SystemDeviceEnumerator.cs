using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.Interfaces;

namespace AudioSwitcher.AudioApi.System
{
    /// <summary>
    ///     Enumerates Windwows System Devices.
    ///     Stores the current devices in memory to avoid calling the COM library when not requried
    /// </summary>
    [ComVisible(false)]
    public sealed class SystemDeviceEnumerator : IDeviceEnumerator<SystemAudioDevice>, IMMNotificationClient,
        IDisposable
    {
        private readonly object _mutex = new object();
        internal MMDeviceEnumerator InnerEnumerator;
        private ConcurrentBag<SystemAudioDevice> _deviceCache = new ConcurrentBag<SystemAudioDevice>();

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        public SystemDeviceEnumerator()
        {
            InnerEnumerator = new MMDeviceEnumerator();
            InnerEnumerator.RegisterEndpointNotificationCallback(this);
        }

        public AudioController Controller
        {
            get;
            set;
        }

        public SystemAudioDevice DefaultPlaybackDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Render, Role.Console | Role.Multimedia); }
        }

        public SystemAudioDevice DefaultCommunicationsPlaybackDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Render, Role.Communications); }
        }

        public SystemAudioDevice DefaultRecordingDevice
        {
            get { return GetDefaultAudioDevice(DataFlow.Capture, Role.Console | Role.Multimedia); }
        }

        public SystemAudioDevice DefaultCommunicationsRecordingDevice
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
                _deviceCache = new ConcurrentBag<SystemAudioDevice>();
                foreach (MMDevice mDev in InnerEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
                {
                    _deviceCache.Add(new SystemAudioDevice(mDev, this));
                }
            }
        }

        public void OnAudioDeviceChanged(AudioDeviceChangedEventArgs e)
        {
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(e.Device, e);
        }

        #region IDevEnum Members

        public SystemAudioDevice GetAudioDevice(Guid id)
        {
            lock (_mutex)
            {
                RefreshSystemDevices();
                return _deviceCache.FirstOrDefault(x => x.ID == id);
            }
        }

        public bool SetDefaultDevice(AudioDevice dev)
        {
            return SetDefaultDevice(dev as SystemAudioDevice);
        }

        public bool SetDefaultCommunicationsDevice(AudioDevice dev)
        {
            return SetDefaultCommunicationsDevice(dev as SystemAudioDevice);
        }

        public bool SetDefaultDevice(SystemAudioDevice dev)
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

        public bool SetDefaultCommunicationsDevice(SystemAudioDevice dev)
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

        public SystemAudioDevice GetDefaultAudioDevice(DataFlow dataflow, Role eRole)
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

        public IEnumerable<SystemAudioDevice> GetAudioDevices(DataFlow dataflow, DeviceState state)
        {
            return
                InnerEnumerator.EnumerateAudioEndPoints(dataflow, state)
                    .Select((x, y) => new SystemAudioDevice(x, this));
        }

        #endregion

        #region IMMNotif Members

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(SystemAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.StateChanged));
        }

        void IMMNotificationClient.OnDeviceAdded(string deviceId)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(SystemAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Added));
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(SystemAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Removed));
        }

        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string deviceId)
        {
            if (role == Role.Console || role == Role.Multimedia)
                OnAudioDeviceChanged(
                    new AudioDeviceChangedEventArgs(GetAudioDevice(SystemAudioDevice.SystemIDToGuid(deviceId)),
                        AudioDeviceEventType.DefaultDevice));
            else if (role == Role.Communications)
                OnAudioDeviceChanged(
                    new AudioDeviceChangedEventArgs(GetAudioDevice(SystemAudioDevice.SystemIDToGuid(deviceId)),
                        AudioDeviceEventType.DefaultCommunicationsDevice));
        }

        void IMMNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            OnAudioDeviceChanged(
                new AudioDeviceChangedEventArgs(GetAudioDevice(SystemAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

        #endregion
    }
}