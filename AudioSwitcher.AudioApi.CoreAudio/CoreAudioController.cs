using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
    public sealed class CoreAudioController : AudioController<CoreAudioDevice>, ISystemAudioEventClient
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private IMMDeviceEnumerator _innerEnumerator;
        private HashSet<CoreAudioDevice> _deviceCache = new HashSet<CoreAudioDevice>();

        public CoreAudioController()
        {
            ComThread.Invoke(() =>
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                _innerEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;

                if (_innerEnumerator == null)
                    return;

                _notificationClient = new MMNotificationClient(this);
                _innerEnumerator.RegisterEndpointNotificationCallback(_notificationClient);
            });

            RefreshSystemDevices();
        }

        ~CoreAudioController()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (_innerEnumerator != null)
            {
                ComThread.BeginInvoke(() =>
                {
                    _innerEnumerator.UnregisterEndpointNotificationCallback(_notificationClient);
                    _notificationClient = null;
                    _innerEnumerator = null;
                });
            }
            _deviceCache = null;

            if (_lock != null)
                _lock.Dispose();

            GC.SuppressFinalize(this);
        }

        public override CoreAudioDevice GetDevice(Guid id, DeviceState state)
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return _deviceCache.FirstOrDefault(x => x.Id == id && state.HasFlag(x.State));
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        private void RefreshSystemDevices()
        {
            ComThread.Invoke(() =>
            {
                _deviceCache = new HashSet<CoreAudioDevice>();
                IMMDeviceCollection collection;
                _innerEnumerator.EnumAudioEndpoints(EDataFlow.All, EDeviceState.All, out collection);

                using (var coll = new MMDeviceCollection(collection))
                {
                    foreach (var mDev in coll)
                        CacheDevice(mDev);
                }
            });

            //Have to collect here to reduce the memory/handle leak issue in Windows 8 and above
            //GC.Collect();
        }

        private void AddDeviceFromRealId(string deviceId)
        {
            ComThread.Invoke(() =>
            {
                IMMDevice mDevice;
                _innerEnumerator.GetDevice(deviceId, out mDevice);

                if (mDevice != null)
                    CacheDevice(mDevice);
            });
        }

        private void RemoveDeviceFromRealId(string deviceId)
        {
            var lockAcquired = _lock.AcquireWriteLockNonReEntrant();
            try
            {
                _deviceCache.RemoveWhere(
                    x => String.Equals(x.RealId, deviceId, StringComparison.InvariantCultureIgnoreCase));
            }
            finally
            {
                if (lockAcquired)
                    _lock.ExitWriteLock();
            }
        }

        private CoreAudioDevice CacheDevice(IMMDevice mDevice)
        {
            if (!DeviceIsValid(mDevice))
                return null;

            string id;
            mDevice.GetId(out id);
            var device = _deviceCache.FirstOrDefault(x => String.Equals(x.RealId, id, StringComparison.InvariantCultureIgnoreCase));

            if (device != null)
                return device;

            var lockAcquired = _lock.AcquireWriteLockNonReEntrant();

            try
            {
                device = new CoreAudioDevice(mDevice, this);
                _deviceCache.Add(device);
                return device;
            }
            finally
            {
                if (lockAcquired)
                    _lock.ExitWriteLock();
            }
        }

        private static bool DeviceIsValid(IMMDevice device)
        {
            try
            {
                string id;
                EDeviceState state;
                device.GetId(out id);
                device.GetState(out state);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RaiseAudioDeviceChanged(AudioDeviceChangedEventArgs e)
        {
            OnAudioDeviceChanged(this, e);
        }

        public override CoreAudioDevice GetDevice(Guid id)
        {
            return GetDevice(id, DeviceState.All);
        }

        public override bool SetDefaultDevice(IDevice dev)
        {
            return SetDefaultDevice(dev as CoreAudioDevice);
        }

        public override bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            return SetDefaultCommunicationsDevice(dev as CoreAudioDevice);
        }

        public override bool SetDefaultDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            try
            {
                if (IsNotVista())
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

        private static bool IsNotVista()
        {
            return Environment.OSVersion.Version.Major > 6
                   || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);
        }

        public override bool SetDefaultCommunicationsDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            try
            {
                if (IsNotVista())
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

        public override CoreAudioDevice GetDefaultDevice(DeviceType deviceType, Role role)
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                IMMDevice dev;
                _innerEnumerator.GetDefaultAudioEndpoint(deviceType.AsEDataFlow(), role.AsERole(), out dev);
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
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        public override IEnumerable<CoreAudioDevice> GetDevices(DeviceType deviceType, DeviceState state)
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return _deviceCache.Where(x =>
                    (x.DeviceType == deviceType || deviceType == DeviceType.All)
                    && state.HasFlag(x.State));
            }
            finally
            {
                if (acquiredLock)
                    _lock.ExitReadLock();
            }
        }

        private MMNotificationClient _notificationClient;

        void ISystemAudioEventClient.OnDeviceStateChanged(string deviceId, EDeviceState newState)
        {
            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.StateChanged));
        }

        void ISystemAudioEventClient.OnDeviceAdded(string deviceId)
        {
            AddDeviceFromRealId(deviceId);

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.Added));
        }

        void ISystemAudioEventClient.OnDeviceRemoved(string deviceId)
        {
            RemoveDeviceFromRealId(deviceId);

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.Removed));
        }

        void ISystemAudioEventClient.OnDefaultDeviceChanged(EDataFlow flow, ERole role, string deviceId)
        {
            Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("Default Device Changed ThreadId : " + Thread.CurrentThread.ManagedThreadId);
                AudioDeviceEventType eventType;

                if (role == ERole.Console || role == ERole.Multimedia)
                    eventType = AudioDeviceEventType.DefaultDevice;
                else
                    eventType = AudioDeviceEventType.DefaultCommunicationsDevice;

                RaiseAudioDeviceChanged(
                    new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)), eventType));
            });
        }

        void ISystemAudioEventClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            RefreshSystemDevices();

            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

    }
}