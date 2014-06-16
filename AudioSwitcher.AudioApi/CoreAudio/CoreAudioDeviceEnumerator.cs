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
        private Object _mutex = new Object();
        internal MMDeviceEnumerator InnerEnumerator;
        private ConcurrentBag<CoreAudioDevice> _deviceCache = new ConcurrentBag<CoreAudioDevice>();

        public CoreAudioDeviceEnumerator()
        {
            //Invoke on ComThread Synchronously
            ComThread.Invoke(() =>
            {
                InnerEnumerator = new MMDeviceEnumerator();
            });

            InnerEnumerator.RegisterEndpointNotificationCallback(this);

            RefreshSystemDevices();
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        public AudioController AudioController { get; set; }

        public CoreAudioDevice DefaultPlaybackDevice
        {
            get { return GetDefaultDevice(DataFlow.Render, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsPlaybackDevice
        {
            get { return GetDefaultDevice(DataFlow.Render, Role.Communications); }
        }

        public CoreAudioDevice DefaultCaptureDevice
        {
            get { return GetDefaultDevice(DataFlow.Capture, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsCaptureDevice
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

        Device IDeviceEnumerator.DefaultCaptureDevice
        {
            get { return DefaultCaptureDevice; }
        }

        Device IDeviceEnumerator.DefaultCommunicationsCaptureDevice
        {
            get { return DefaultCommunicationsCaptureDevice; }
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
                ComThread.Invoke(() =>
                {
                    _deviceCache = new ConcurrentBag<CoreAudioDevice>();
                    foreach (var mDev in InnerEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
                    {
                        var dev = new CoreAudioDevice(mDev, this);
                        _deviceCache.Add(dev);
                    }
                });
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
                    PolicyConfig.SetDefaultEndpoint(dev.RealId, Role.Console | Role.Multimedia);
                else
                    PolicyConfigVista.SetDefaultEndpoint(dev.RealId, Role.Console | Role.Multimedia);

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
                    PolicyConfig.SetDefaultEndpoint(dev.RealId, Role.Communications);
                else
                    PolicyConfigVista.SetDefaultEndpoint(dev.RealId, Role.Communications);

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
                MMDevice defDev = InnerEnumerator.GetDefaultAudioEndpoint(dataflow, eRole);
                if (defDev == null || string.IsNullOrEmpty(defDev.ID))
                    return null;

                return _deviceCache.First(x => x.RealId == defDev.ID);
            }
        }

        public IEnumerable<CoreAudioDevice> GetDevices(DataFlow dataflow, DeviceState state)
        {
            lock (_mutex)
            {
                return _deviceCache.Where(x => 
                    (x.DataFlow == dataflow || dataflow == DataFlow.All)
                    && (x.State & state) > 0);
            }
        }

        #endregion

        #region IMMNotif Members

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.StateChanged));
        }

        void IMMNotificationClient.OnDeviceAdded(string deviceId)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.Added));
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
            RefreshSystemDevices();

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

            RefreshSystemDevices();

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
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(this,
                new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIDToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

        #endregion
    }
}