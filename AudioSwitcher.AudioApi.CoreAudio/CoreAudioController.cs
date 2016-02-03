using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     Enumerates Windows System Devices.
    ///     Stores the current devices in memory to avoid calling the COM library when not required
    /// </summary>
    public sealed class CoreAudioController : AudioController<CoreAudioDevice>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private HashSet<CoreAudioDevice> _deviceCache = new HashSet<CoreAudioDevice>();
        private IMultimediaDeviceEnumerator _innerEnumerator;
        private SystemEventNotifcationClient _systemEvents;

        public CoreAudioController()
        {
            ComThread.Invoke(() =>
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                _innerEnumerator = ComObjectFactory.GetDeviceEnumerator();

                if (_innerEnumerator == null)
                    return;

                _systemEvents = new SystemEventNotifcationClient(_innerEnumerator);

                _systemEvents.DeviceAdded.Subscribe(x => OnDeviceAdded(x.DeviceId));
                _systemEvents.DeviceRemoved.Subscribe(x => OnDeviceRemoved(x.DeviceId));
            });

            RefreshSystemDevices();
        }

        internal SystemEventNotifcationClient SystemEvents
        {
            get
            {
                return _systemEvents;
            }
        }

        void OnDeviceAdded(string deviceId)
        {
            var dev = GetOrAddDeviceFromRealId(deviceId);

            if (dev != null)
                OnAudioDeviceChanged(new DeviceAddedArgs(dev));
        }

        void OnDeviceRemoved(string deviceId)
        {
            var devicesRemoved = RemoveFromRealId(deviceId);

            foreach (var dev in devicesRemoved)
                OnAudioDeviceChanged(new DeviceRemovedArgs(dev));
        }

        ~CoreAudioController()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            ComThread.BeginInvoke(() =>
            {
                if (_systemEvents != null)
                {
                    _systemEvents.Dispose();
                    _systemEvents = null;
                }

                _innerEnumerator = null;
            })
            .ContinueWith(x =>
            {
                if (_deviceCache != null)
                    _deviceCache.Clear();

                if (_lock != null)
                    _lock.Dispose();
            });

            base.Dispose(disposing);

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

        private CoreAudioDevice GetDevice(string realId)
        {
            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
                return
                    _deviceCache.FirstOrDefault(
                        x => string.Equals(x.RealId, realId, StringComparison.InvariantCultureIgnoreCase));
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
                IMultimediaDeviceCollection collection;
                _innerEnumerator.EnumAudioEndpoints(EDataFlow.All, EDeviceState.All, out collection);

                using (var coll = new MultimediaDeviceCollection(collection))
                {
                    foreach (var mDev in coll)
                        CacheDevice(mDev);
                }
            });
        }

        private CoreAudioDevice GetOrAddDeviceFromRealId(string deviceId)
        {
            //This pre-check here may prevent more com objects from being created
            var device = GetDevice(deviceId);
            if (device != null)
                return device;

            return ComThread.Invoke(() =>
            {
                IMultimediaDevice mDevice;
                _innerEnumerator.GetDevice(deviceId, out mDevice);

                if (mDevice == null)
                    return null;

                return CacheDevice(mDevice);
            });
        }

        private IEnumerable<CoreAudioDevice> RemoveFromRealId(string deviceId)
        {
            var lockAcquired = _lock.AcquireWriteLockNonReEntrant();
            try
            {
                var devicesToRemove =
                    _deviceCache.Where(
                        x => string.Equals(x.RealId, deviceId, StringComparison.InvariantCultureIgnoreCase)).ToList();

                _deviceCache.RemoveWhere(
                    x => string.Equals(x.RealId, deviceId, StringComparison.InvariantCultureIgnoreCase));

                return devicesToRemove;
            }
            finally
            {
                if (lockAcquired)
                    _lock.ExitWriteLock();
            }
        }

        private CoreAudioDevice CacheDevice(IMultimediaDevice mDevice)
        {
            if (!DeviceIsValid(mDevice))
                return null;

            string id;
            mDevice.GetId(out id);
            var device = GetDevice(id);

            if (device != null)
                return device;

            device = new CoreAudioDevice(mDevice, this);

            device.StateChanged.Subscribe(OnAudioDeviceChanged);
            device.DefaultChanged.Subscribe(OnAudioDeviceChanged);
            device.PropertyChanged.Subscribe(OnAudioDeviceChanged);

            var lockAcquired = _lock.AcquireWriteLockNonReEntrant();

            try
            {
                _deviceCache.Add(device);
                return device;
            }
            finally
            {
                if (lockAcquired)
                    _lock.ExitWriteLock();
            }
        }

        private static bool DeviceIsValid(IMultimediaDevice device)
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

        public override bool SetDefaultDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            var oldDefault = dev.IsPlaybackDevice ? DefaultPlaybackDevice : DefaultCaptureDevice;

            try
            {
                PolicyConfig.SetDefaultEndpoint(dev.RealId, ERole.Console | ERole.Multimedia);
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
                    OnAudioDeviceChanged(new DefaultDeviceChangedArgs(oldDefault));
            }
        }

        public override bool SetDefaultCommunicationsDevice(CoreAudioDevice dev)
        {
            if (dev == null)
                return false;

            var oldDefault = dev.IsPlaybackDevice
                ? DefaultPlaybackCommunicationsDevice
                : DefaultCaptureCommunicationsDevice;

            try
            {
                PolicyConfig.SetDefaultEndpoint(dev.RealId, ERole.Communications);
                return dev.IsDefaultCommunicationsDevice;
            }
            catch
            {
                return false;
            }
            finally
            {
                //Raise the default changed event on the old device
                if (oldDefault != null && !oldDefault.IsDefaultCommunicationsDevice)
                    OnAudioDeviceChanged(new DefaultDeviceChangedArgs(oldDefault));
            }
        }

        public override CoreAudioDevice GetDefaultDevice(DeviceType deviceType, Role role)
        {
            IMultimediaDevice dev;
            _innerEnumerator.GetDefaultAudioEndpoint(deviceType.AsEDataFlow(), role.AsERole(), out dev);
            if (dev == null)
                return null;

            string devId;
            dev.GetId(out devId);
            if (string.IsNullOrEmpty(devId))
                return null;

            var acquiredLock = _lock.AcquireReadLockNonReEntrant();

            try
            {
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
    }
}