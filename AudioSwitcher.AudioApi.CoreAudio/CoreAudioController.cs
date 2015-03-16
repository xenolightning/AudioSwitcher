using System;
using System.Collections.Generic;
using System.Linq;
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

        private void CacheDevice(IMMDevice mDevice)
        {
            if (!DeviceIsValid(mDevice))
                return;

            string id;
            mDevice.GetId(out id);
            var device = _deviceCache.FirstOrDefault(x => String.Equals(x.RealId, id, StringComparison.InvariantCultureIgnoreCase));

            if (device != null)
                return;

            var lockAcquired = _lock.AcquireWriteLockNonReEntrant();

            try
            {
                device = new CoreAudioDevice(mDevice, this);
                _deviceCache.Add(device);
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

        public override bool SetDefaultDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            var oldDefault = dev.IsPlaybackDevice ? DefaultPlaybackDevice : DefaultCaptureDevice;

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
            finally
            {
                //Raise the default changed event on the old device
                if (oldDefault != null && !oldDefault.IsDefaultDevice)
                    RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(oldDefault, AudioDeviceEventType.DefaultDevice));
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

            var oldDefault = dev.IsPlaybackDevice ? DefaultPlaybackCommunicationsDevice : DefaultCaptureCommunicationsDevice;

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
            finally
            {
                //Raise the default changed event on the old device
                if(oldDefault != null && !oldDefault.IsDefaultCommunicationsDevice)
                    RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(oldDefault, AudioDeviceEventType.DefaultCommunicationsDevice));
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
                    && state.HasFlag(x.State)).ToList();
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
            //Ignore multimedia, it seems to fire a console event anyway
            if (role == ERole.Multimedia)
                return;

            Task.Factory.StartNew(() =>
            {
                var eventType = role == ERole.Console ? AudioDeviceEventType.DefaultDevice : AudioDeviceEventType.DefaultCommunicationsDevice;

                RaiseAudioDeviceChanged(
                    new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)), eventType));
            });
        }

        void ISystemAudioEventClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            RaiseAudioDeviceChanged(new AudioDeviceChangedEventArgs(GetDevice(CoreAudioDevice.SystemIdToGuid(deviceId)),
                    AudioDeviceEventType.PropertyChanged));
        }

    }
}