using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ReaderWriterLockSlim _refreshLock = new ReaderWriterLockSlim();

        private IMMDeviceEnumerator _innerEnumerator;
        private ConcurrentBag<CoreAudioDevice> _deviceCache = new ConcurrentBag<CoreAudioDevice>();

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
            if (disposing)
            {
                if (_innerEnumerator != null)
                {
                    ComThread.BeginInvoke(() =>
                    {
                        _innerEnumerator.UnregisterEndpointNotificationCallback(_notificationClient);
                        _notificationClient = null;
                    });
                }
                _deviceCache = null;
                _innerEnumerator = null;
                _lock.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public override CoreAudioDevice GetDevice(Guid id, DeviceState state)
        {
            _lock.EnterReadLock();
            try
            {
                return _deviceCache.FirstOrDefault(x => x.Id == id && state.HasFlag(x.State));
            }
            finally
            {
                _lock.ExitReadLock();
            }
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
                            if (!DeviceIsValid(mDev))
                                continue;

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

        public override bool SetDefaultCommunicationsDevice(CoreAudioDevice dev)
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

        public override CoreAudioDevice GetDefaultDevice(DeviceType deviceType, Role role)
        {
            _lock.EnterReadLock();
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
                _lock.ExitReadLock();
            }
        }

        public override IEnumerable<CoreAudioDevice> GetDevices(DeviceType deviceType, DeviceState state)
        {
            _lock.EnterReadLock();
            try
            {
                return _deviceCache.Where(x =>
                    (x.DeviceType == deviceType || deviceType == DeviceType.All)
                    && state.HasFlag(x.State));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private MMNotificationClient _notificationClient;

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

                    Debug.Write(Thread.CurrentThread.ManagedThreadId);

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

    }
}