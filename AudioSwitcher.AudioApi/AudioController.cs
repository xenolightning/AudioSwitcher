using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioController : IAudioController
    {
        protected const DeviceState DEFAULT_DEVICE_STATE_FILTER =
            DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled;

        private readonly AsyncBroadcaster<DeviceChangedArgs> _audioDeviceChanged;

        protected AudioController()
        {
            _audioDeviceChanged = new AsyncBroadcaster<DeviceChangedArgs>();
        }

        public virtual IDevice DefaultPlaybackDevice
        {
            get
            {
                return GetDefaultDevice(DeviceType.Playback, Role.Console | Role.Multimedia);
            }
            set
            {
                SetDefaultDevice(value);
            }
        }

        public virtual IDevice DefaultPlaybackCommunicationsDevice
        {
            get
            {
                return GetDefaultDevice(DeviceType.Playback, Role.Communications);
            }
            set
            {
                SetDefaultCommunicationsDevice(value);
            }
        }

        public virtual IDevice DefaultCaptureDevice
        {
            get
            {
                return GetDefaultDevice(DeviceType.Capture, Role.Console | Role.Multimedia);
            }
            set
            {
                SetDefaultDevice(value);
            }
        }

        public virtual IDevice DefaultCaptureCommunicationsDevice
        {
            get
            {
                return GetDefaultDevice(DeviceType.Capture, Role.Communications);
            }
            set
            {
                SetDefaultCommunicationsDevice(value);
            }
        }

        public IObservable<DeviceChangedArgs> AudioDeviceChanged
        {
            get { return _audioDeviceChanged.AsObservable(); }
        }

        public abstract IDevice GetDevice(Guid id);

        public virtual Task<IDevice> GetDeviceAsync(Guid id)
        {
            return Task.FromResult(GetDevice(id));
        }

        public abstract IDevice GetDevice(Guid id, DeviceState state);

        public virtual Task<IDevice> GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.FromResult(GetDevice(id, state));
        }

        public abstract IDevice GetDefaultDevice(DeviceType deviceType, Role role);

        public virtual Task<IDevice> GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return Task.FromResult(GetDefaultDevice(deviceType, role));
        }

        public virtual IEnumerable<IDevice> GetDevices()
        {
            return GetDevices(DEFAULT_DEVICE_STATE_FILTER);
        }

        public virtual Task<IEnumerable<IDevice>> GetDevicesAsync()
        {
            return Task.FromResult(GetDevices());
        }

        public virtual Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceState state)
        {
            return Task.FromResult(GetDevices(state));
        }

        public IEnumerable<IDevice> GetDevices(DeviceType deviceType)
        {
            return GetDevices(deviceType, DEFAULT_DEVICE_STATE_FILTER);
        }

        public Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceType deviceType)
        {
            return GetDevicesAsync(deviceType, DEFAULT_DEVICE_STATE_FILTER);
        }

        public virtual IEnumerable<IDevice> GetDevices(DeviceState state)
        {
            return GetDevices(DeviceType.All, state);
        }

        public abstract IEnumerable<IDevice> GetDevices(DeviceType deviceType, DeviceState state);

        public virtual Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            return Task.FromResult(GetDevices(deviceType, state));
        }

        public abstract IEnumerable<IDevice> GetPlaybackDevices();

        public virtual Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync()
        {
            return Task.FromResult(GetPlaybackDevices());
        }

        public abstract IEnumerable<IDevice> GetPlaybackDevices(DeviceState deviceState);

        public virtual Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync(DeviceState deviceState)
        {
            return Task.FromResult(GetPlaybackDevices(deviceState));
        }

        public abstract IEnumerable<IDevice> GetCaptureDevices();

        public virtual Task<IEnumerable<IDevice>> GetCaptureDevicesAsync()
        {
            return Task.FromResult(GetCaptureDevices());
        }

        public abstract IEnumerable<IDevice> GetCaptureDevices(DeviceState deviceState);

        public virtual Task<IEnumerable<IDevice>> GetCaptureDevicesAsync(DeviceState deviceState)
        {
            return Task.FromResult(GetCaptureDevices(deviceState));
        }

        public abstract bool SetDefaultDevice(IDevice dev);

        public virtual Task<bool> SetDefaultDeviceAsync(IDevice dev)
        {
            return Task.FromResult(SetDefaultDevice(dev));
        }

        public abstract bool SetDefaultCommunicationsDevice(IDevice dev);

        public virtual Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev)
        {
            return Task.FromResult(SetDefaultDevice(dev));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void OnAudioDeviceChanged(object sender, DeviceChangedArgs e)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}