using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioController : IAudioController
    {
        protected const DeviceState DefaultDeviceStateFilter =
            DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled;

        private readonly Broadcaster<DeviceChangedArgs> _audioDeviceChanged;

        protected AudioController()
        {
            _audioDeviceChanged = new Broadcaster<DeviceChangedArgs>();
        }

        public virtual IDevice DefaultPlaybackDevice => GetDefaultDevice(DeviceType.Playback, Role.Console | Role.Multimedia);

        public virtual IDevice DefaultPlaybackCommunicationsDevice => GetDefaultDevice(DeviceType.Playback, Role.Communications);

        public virtual IDevice DefaultCaptureDevice => GetDefaultDevice(DeviceType.Capture, Role.Console | Role.Multimedia);

        public virtual IDevice DefaultCaptureCommunicationsDevice => GetDefaultDevice(DeviceType.Capture, Role.Communications);

        public IObservable<DeviceChangedArgs> AudioDeviceChanged => _audioDeviceChanged.AsObservable();

        public abstract IDevice GetDevice(Guid id);

        public virtual Task<IDevice> GetDeviceAsync(Guid id)
        {
            return TaskShim.FromResult(GetDevice(id));
        }

        public abstract IDevice GetDevice(Guid id, DeviceState state);

        public virtual Task<IDevice> GetDeviceAsync(Guid id, DeviceState state)
        {
            return TaskShim.FromResult(GetDevice(id, state));
        }

        public abstract IDevice GetDefaultDevice(DeviceType deviceType, Role role);

        public virtual Task<IDevice> GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return TaskShim.FromResult(GetDefaultDevice(deviceType, role));
        }

        public virtual IEnumerable<IDevice> GetDevices()
        {
            return GetDevices(DefaultDeviceStateFilter);
        }

        public virtual Task<IEnumerable<IDevice>> GetDevicesAsync()
        {
            return TaskShim.FromResult(GetDevices());
        }

        public virtual Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceState state)
        {
            return TaskShim.FromResult(GetDevices(state));
        }

        public IEnumerable<IDevice> GetDevices(DeviceType deviceType)
        {
            return GetDevices(deviceType, DefaultDeviceStateFilter);
        }

        public Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceType deviceType)
        {
            return GetDevicesAsync(deviceType, DefaultDeviceStateFilter);
        }

        public virtual IEnumerable<IDevice> GetDevices(DeviceState state)
        {
            return GetDevices(DeviceType.All, state);
        }

        public abstract IEnumerable<IDevice> GetDevices(DeviceType deviceType, DeviceState state);

        public virtual Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            return TaskShim.FromResult(GetDevices(deviceType, state));
        }

        public abstract IEnumerable<IDevice> GetPlaybackDevices();

        public virtual Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync()
        {
            return TaskShim.FromResult(GetPlaybackDevices());
        }

        public abstract IEnumerable<IDevice> GetPlaybackDevices(DeviceState deviceState);

        public virtual Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync(DeviceState deviceState)
        {
            return TaskShim.FromResult(GetPlaybackDevices(deviceState));
        }

        public abstract IEnumerable<IDevice> GetCaptureDevices();

        public virtual Task<IEnumerable<IDevice>> GetCaptureDevicesAsync()
        {
            return TaskShim.FromResult(GetCaptureDevices());
        }

        public abstract IEnumerable<IDevice> GetCaptureDevices(DeviceState deviceState);

        public virtual Task<IEnumerable<IDevice>> GetCaptureDevicesAsync(DeviceState deviceState)
        {
            return TaskShim.FromResult(GetCaptureDevices(deviceState));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void OnAudioDeviceChanged(DeviceChangedArgs e)
        {
            _audioDeviceChanged.OnNext(e);
        }

        protected virtual void Dispose(bool disposing)
        {
            _audioDeviceChanged.Dispose();
        }
    }
}