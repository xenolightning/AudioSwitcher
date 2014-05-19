using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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

        public Controller Controller { get; set; }

        public CoreAudioDevice DefaultPlaybackDevice
        {
            get { return GetDefaultDevice(DataFlow.Render, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsPlaybackDevice
        {
            get { return GetDefaultDevice(DataFlow.Render, Role.Communications); }
        }

        public CoreAudioDevice DefaultRecordingDevice
        {
            get { return GetDefaultDevice(DataFlow.Capture, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsRecordingDevice
        {
            get { return GetDefaultDevice(DataFlow.Capture, Role.Communications); }
        }

        Device IDeviceEnumerator.DefaultPlaybackDevice
        {
            get { return DefaultPlaybackDevice; }
        }

        Device IDeviceEnumerator.DefaultCommunicationsPlaybackDevice
        {
            get { return DefaultCommunicationsPlaybackDevice; }
        }

        Device IDeviceEnumerator.DefaultRecordingDevice
        {
            get { return DefaultRecordingDevice; }
        }

        Device IDeviceEnumerator.DefaultCommunicationsRecordingDevice
        {
            get { return DefaultCommunicationsRecordingDevice; }
        }

        Device IDeviceEnumerator.GetDevice(Guid id)
        {
            return GetDevice(id);
        }

        Device IDeviceEnumerator.GetDefaultDevice(DataFlow dataflow, Role eRole)
        {
            return GetDefaultDevice(dataflow, eRole);
        }

        IEnumerable<Device> IDeviceEnumerator.GetDevices(DataFlow dataflow, DeviceState eRole)
        {
            return GetDevices(dataflow, eRole);
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
                foreach (var mDev in InnerEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
                {
                    var dev = new CoreAudioDevice(mDev, this);
                    _deviceCache.Add(dev);
                }
            }
        }

        public void RaiseAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(this, e);
        }

        #region IDevEnum Members

        public CoreAudioDevice GetDevice(Guid id)
        {
            lock (_mutex)
            {
                RefreshSystemDevices();
                return _deviceCache.FirstOrDefault(x => x.Id == id);
            }
        }

        public bool SetDefaultDevice(Device dev)
        {
            return SetDefaultDevice(dev as CoreAudioDevice);
        }

        public bool SetDefaultCommunicationsDevice(Device dev)
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

        public CoreAudioDevice GetDefaultDevice(DataFlow dataflow, Role eRole)
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

        public IEnumerable<CoreAudioDevice> GetDevices(DataFlow dataflow, DeviceState state)
        {
            return
                InnerEnumerator.EnumerateAudioEndPoints(dataflow, state)
                    .Select((x, y) => new CoreAudioDevice(x, this));
        }

        #endregion

        #region IMMNotif Members

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.StateChanged));
        }

        void IMMNotificationClient.OnDeviceAdded(string deviceId)
        {
            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Added));
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Removed));
        }


        readonly ConcurrentDictionary<string, bool> _processingIds = new ConcurrentDictionary<string, bool>();

        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string deviceId)
        {
            //Need to do some event filtering here, there's a scenario where
            //multiple default device changed are raised when one playback device changes.
            //This is correct functionality, but I want to limit it to one event per device
            //this is specific to Audio Switcher only. You could theorectically want an event 
            //signalling a non default then a default update

            lock (_processingIds)
            {
                if (_processingIds.ContainsKey(deviceId))
                    return;

                _processingIds.TryAdd(deviceId, true);
            }

            if (role == Role.Console || role == Role.Multimedia)
                RaiseAudioDeviceChanged(this,
                    new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                        AudioDeviceEventType.DefaultDevice));
            else if (role == Role.Communications)
                RaiseAudioDeviceChanged(this,
                    new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                        AudioDeviceEventType.DefaultCommunicationsDevice));

            bool temp;
            _processingIds.TryRemove(deviceId, out temp);
        }

        void IMMNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

        #endregion
    }
}