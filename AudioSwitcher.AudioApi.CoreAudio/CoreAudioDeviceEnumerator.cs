using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     Enumerates Windows System Devices.
    ///     Stores the current devices in memory to avoid calling the COM library when not required
    /// </summary>
    public sealed class CoreAudioDeviceEnumerator : IDeviceEnumerator<CoreAudioDevice>, ISystemAudioEventClient
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _refreshLock = new ReaderWriterLockSlim();

        private IMMDeviceEnumerator _innerEnumerator;
        private ConcurrentBag<CoreAudioDevice> _deviceCache = new ConcurrentBag<CoreAudioDevice>();

        public CoreAudioDeviceEnumerator()
        {

            ComThread.Invoke(() =>
            {
                _innerEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
            });

            _notificationClient = new MMNotificationClient(this);

            _innerEnumerator.RegisterEndpointNotificationCallback(_notificationClient);

            RefreshSystemDevices();
        }

        ~CoreAudioDeviceEnumerator()
        {
            Dispose(false);
        }

        public IAudioController AudioController
        {
            get;
            set;
        }

        public CoreAudioDevice DefaultPlaybackDevice
        {
            get { return GetDefaultDevice(DeviceType.Playback, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsPlaybackDevice
        {
            get { return GetDefaultDevice(DeviceType.Playback, Role.Communications); }
        }

        public CoreAudioDevice DefaultCaptureDevice
        {
            get { return GetDefaultDevice(DeviceType.Capture, Role.Console | Role.Multimedia); }
        }

        public CoreAudioDevice DefaultCommunicationsCaptureDevice
        {
            get { return GetDefaultDevice(DeviceType.Capture, Role.Communications); }
        }

        IDevice IDeviceEnumerator.DefaultPlaybackDevice
        {
            get { return DefaultPlaybackDevice; }
        }

        IDevice IDeviceEnumerator.DefaultCommunicationsPlaybackDevice
        {
            get { return DefaultCommunicationsPlaybackDevice; }
        }

        IDevice IDeviceEnumerator.DefaultCaptureDevice
        {
            get { return DefaultCaptureDevice; }
        }

        IDevice IDeviceEnumerator.DefaultCommunicationsCaptureDevice
        {
            get { return DefaultCommunicationsCaptureDevice; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_innerEnumerator != null)
                {
                    ComThread.BeginInvoke(() =>
                    {
                        _innerEnumerator.UnregisterEndpointNotificationCallback(_notificationClient);
                    });
                }
                _deviceCache = null;
                _innerEnumerator = null;
            }

            GC.SuppressFinalize(this);
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        IDevice IDeviceEnumerator.GetDevice(Guid id)
        {
            return GetDevice(id);
        }

        public CoreAudioDevice GetDevice(Guid id, DeviceState state)
        {
            _lock.EnterReadLock();
            try
            {
                return _deviceCache.FirstOrDefault(x => x.Id == id && (x.State & state) > 0);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Task<CoreAudioDevice> GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevice(id, state));
        }

        IDevice IDeviceEnumerator.GetDevice(Guid id, DeviceState state)
        {
            return GetDevice(id, state);
        }

        public Task<CoreAudioDevice> GetDeviceAsync(Guid id)
        {
            return Task.Factory.StartNew(() => GetDevice(id));
        }

        Task<IDevice> IDeviceEnumerator.GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevice(id, state) as IDevice);
        }

        Task<CoreAudioDevice> IDeviceEnumerator<CoreAudioDevice>.GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return Task.Factory.StartNew(() => GetDefaultDevice(deviceType, role));
        }

        Task<IEnumerable<CoreAudioDevice>> IDeviceEnumerator<CoreAudioDevice>.GetDevicesAsync(DeviceType deviceType,
            DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevices(deviceType, state));
        }

        IDevice IDeviceEnumerator.GetDefaultDevice(DeviceType type, Role eRole)
        {
            return GetDefaultDevice(type, eRole);
        }

        IEnumerable<IDevice> IDeviceEnumerator.GetDevices(DeviceType type, DeviceState eRole)
        {
            return GetDevices(type, eRole);
        }

        Task<IDevice> IDeviceEnumerator.GetDeviceAsync(Guid id)
        {
            return Task.Factory.StartNew(() => GetDevice(id) as IDevice);
        }

        Task<IDevice> IDeviceEnumerator.GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return Task.Factory.StartNew(() => GetDefaultDevice(deviceType, role) as IDevice);
        }

        Task<IEnumerable<IDevice>> IDeviceEnumerator.GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            // Required for the task return type
            // ReSharper disable once RedundantEnumerableCastCall
            return Task.Factory.StartNew(() => GetDevices(deviceType, state).Cast<IDevice>());
        }

        private void RefreshSystemDevices()
        {
            _lock.EnterWriteLock();
            try
            {
                ComThread.Invoke(() =>
                {
                    _deviceCache = new ConcurrentBag<CoreAudioDevice>();
                    IMMDeviceCollection collection;
                    _innerEnumerator.EnumAudioEndpoints(EDataFlow.All, EDeviceState.All, out collection);
                    using (var coll = new MMDeviceCollection(collection))
                    {
                        foreach (var mDev in coll)
                        {
                            var dev = new CoreAudioDevice(mDev, this);
                            _deviceCache.Add(dev);
                        }
                    }
                });
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            //Have to collect here to reduce the memory/handle leak issue in Windows 8 and above
            GC.Collect();
        }

        private void RaiseAudioDeviceChanged(AudioDeviceChangedEventArgs e)
        {
            var handler = AudioDeviceChanged;

            if (handler != null)
                handler(this, e);
        }

        #region IDevEnum Members

        public CoreAudioDevice GetDevice(Guid id)
        {
            return GetDevice(id, DeviceState.All);
        }

        public bool SetDefaultDevice(IDevice dev)
        {
            return SetDefaultDevice(dev as CoreAudioDevice);
        }

        public bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            return SetDefaultCommunicationsDevice(dev as CoreAudioDevice);
        }

        public Task<bool> SetDefaultDeviceAsync(IDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultDevice(dev));
        }

        public Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultCommunicationsDevice(dev));
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
                    PolicyConfig.SetDefaultEndpoint(dev.RealId, ERole.Console | ERole.Multimedia);
                else
                    PolicyConfigVista.SetDefaultEndpoint(dev.RealId, ERole.Console | ERole.Multimedia);

                return dev.IsDefaultDevice;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> SetDefaultDeviceAsync(CoreAudioDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultDevice(dev));
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
                    PolicyConfig.SetDefaultEndpoint(dev.RealId, ERole.Communications);
                else
                    PolicyConfigVista.SetDefaultEndpoint(dev.RealId, ERole.Communications);

                return dev.IsDefaultCommunicationsDevice;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> SetDefaultCommunicationsDeviceAsync(CoreAudioDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultCommunicationsDevice(dev));
        }

        public CoreAudioDevice GetDefaultDevice(DeviceType deviceType, Role eRole)
        {
            _lock.EnterReadLock();
            try
            {
                IMMDevice dev;
                _innerEnumerator.GetDefaultAudioEndpoint(deviceType.AsEDataFlow(), eRole.AsERole(), out dev);
                if (dev == null)
                    return null;

                string devId;
                dev.GetId(out devId);
                if (String.IsNullOrEmpty(devId))
                    return null;

                return _deviceCache.FirstOrDefault(x => x.RealId == devId);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IEnumerable<CoreAudioDevice> GetDevices(DeviceType deviceType, DeviceState state)
        {
            _lock.EnterReadLock();
            try
            {
                return _deviceCache.Where(x =>
                    (x.DeviceType == deviceType || deviceType == DeviceType.All)
                    && (x.State & state) > 0);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        #region IMMNotif Members

        private readonly MMNotificationClient _notificationClient;

        void ISystemAudioEventClient.OnDeviceStateChanged(string deviceId, EDeviceState newState)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.StateChanged));
        }

        void ISystemAudioEventClient.OnDeviceAdded(string deviceId)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.Added));
        }

        void ISystemAudioEventClient.OnDeviceRemoved(string deviceId)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.Removed));
        }


        void ISystemAudioEventClient.OnDefaultDeviceChanged(EDataFlow flow, ERole role, string deviceId)
        {
            Task.Factory.StartNew(() =>
            {
                //Need to do some event filtering here, there's a scenario where
                //multiple default device changed are raised when one playback device changes.
                //This is correct functionality, but I want to limit it to one event per device
                //this is specific to Audio Switcher only. You could theorectically want an event 
                //signalling a non default then a default update

                try
                {
                    if (!_refreshLock.TryEnterWriteLock(0))
                        return;

                    RefreshSystemDevices();

                    if (role == ERole.Console || role == ERole.Multimedia)
                        RaiseAudioDeviceChanged(
                            new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                                AudioDeviceEventType.DefaultDevice));
                    else if (role == ERole.Communications)
                        RaiseAudioDeviceChanged(
                            new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                                AudioDeviceEventType.DefaultCommunicationsDevice));
                }
                finally
                {
                    if (_refreshLock.IsWriteLockHeld)
                        _refreshLock.ExitWriteLock();
                }
            });
        }

        void ISystemAudioEventClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

        #endregion
    }
}