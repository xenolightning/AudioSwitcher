using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioController<T> : IAudioController<T>
        where T : class, IDevice
    {
        private const DeviceState DefaultDeviceStateFilter = DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled;

        private readonly Broadcaster<DeviceChangedArgs> _audioDeviceChanged;

        protected AudioController()
        {
            _audioDeviceChanged = new Broadcaster<DeviceChangedArgs>();
        }

        public virtual T DefaultPlaybackDevice => GetDefaultDevice(DeviceType.Playback, Role.Console | Role.Multimedia);

        public virtual T DefaultPlaybackCommunicationsDevice => GetDefaultDevice(DeviceType.Playback, Role.Communications);

        public virtual T DefaultCaptureDevice => GetDefaultDevice(DeviceType.Capture, Role.Console | Role.Multimedia);

        public virtual T DefaultCaptureCommunicationsDevice => GetDefaultDevice(DeviceType.Capture, Role.Communications);

        IDevice IAudioController.DefaultPlaybackDevice => DefaultPlaybackDevice;

        IDevice IAudioController.DefaultPlaybackCommunicationsDevice => DefaultPlaybackCommunicationsDevice;

        IDevice IAudioController.DefaultCaptureDevice => DefaultCaptureDevice;

        IDevice IAudioController.DefaultCaptureCommunicationsDevice => DefaultCaptureCommunicationsDevice;

        public IObservable<DeviceChangedArgs> AudioDeviceChanged => _audioDeviceChanged.AsObservable();

        public virtual T GetDevice(Guid id)
        {
            return GetDevice(id, DefaultDeviceStateFilter);
        }

        public virtual Task<T> GetDeviceAsync(Guid id)
        {
            return TaskShim.FromResult(GetDevice(id));
        }

        public abstract T GetDevice(Guid id, DeviceState state);

        public virtual Task<T> GetDeviceAsync(Guid id, DeviceState state)
        {
            return TaskShim.FromResult(GetDevice(id, state));
        }

        public abstract T GetDefaultDevice(DeviceType deviceType, Role role);

        public virtual Task<T> GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return TaskShim.FromResult(GetDefaultDevice(deviceType, role));
        }

        public virtual IEnumerable<T> GetDevices()
        {
            return GetDevices(DefaultDeviceStateFilter);
        }

        public virtual Task<IEnumerable<T>> GetDevicesAsync()
        {
            return TaskShim.FromResult(GetDevices());
        }

        public virtual Task<IEnumerable<T>> GetDevicesAsync(DeviceState state)
        {
            return TaskShim.FromResult(GetDevices(state));
        }

        public IEnumerable<T> GetDevices(DeviceType deviceType)
        {
            return GetDevices(deviceType, DefaultDeviceStateFilter);
        }

        public Task<IEnumerable<T>> GetDevicesAsync(DeviceType deviceType)
        {
            return GetDevicesAsync(deviceType, DefaultDeviceStateFilter);
        }

        IEnumerable<IDevice> IAudioController.GetDevices(DeviceType deviceType)
        {
            return GetDevices(deviceType);
        }

        Task<IEnumerable<IDevice>> IAudioController.GetDevicesAsync(DeviceType deviceType)
        {
            return TaskShim.FromResult(GetDevices(deviceType).Cast<IDevice>());
        }

        public virtual IEnumerable<T> GetDevices(DeviceState state)
        {
            return GetDevices(DeviceType.All, state);
        }

        public abstract IEnumerable<T> GetDevices(DeviceType deviceType, DeviceState state);

        public virtual Task<IEnumerable<T>> GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            return TaskShim.FromResult(GetDevices(deviceType, state));
        }

        public virtual IEnumerable<T> GetPlaybackDevices()
        {
            return GetPlaybackDevices(DefaultDeviceStateFilter);
        }

        public virtual IEnumerable<T> GetPlaybackDevices(DeviceState state)
        {
            return GetDevices(DeviceType.Playback, state);
        }

        public virtual Task<IEnumerable<T>> GetPlaybackDevicesAsync()
        {
            return GetPlaybackDevicesAsync(DefaultDeviceStateFilter);
        }

        public virtual Task<IEnumerable<T>> GetPlaybackDevicesAsync(DeviceState deviceState)
        {
            return TaskShim.FromResult(GetPlaybackDevices(deviceState));
        }

        public virtual IEnumerable<T> GetCaptureDevices()
        {
            return GetCaptureDevices(DefaultDeviceStateFilter);
        }

        public virtual Task<IEnumerable<T>> GetCaptureDevicesAsync()
        {
            return TaskShim.FromResult(GetCaptureDevices(DefaultDeviceStateFilter));
        }

        public virtual IEnumerable<T> GetCaptureDevices(DeviceState state)
        {
            return GetDevices(DeviceType.Capture, state);
        }

        public virtual Task<IEnumerable<T>> GetCaptureDevicesAsync(DeviceState deviceState)
        {
            return TaskShim.FromResult(GetCaptureDevices(deviceState));
        }

        Task<IDevice> IAudioController.GetDeviceAsync(Guid id)
        {
            return TaskShim.FromResult(GetDevice(id) as IDevice);
        }

        IDevice IAudioController.GetDevice(Guid id)
        {
            return GetDevice(id);
        }

        IDevice IAudioController.GetDevice(Guid id, DeviceState state)
        {
            return GetDevice(id, state);
        }

        Task<IDevice> IAudioController.GetDeviceAsync(Guid id, DeviceState state)
        {
            return TaskShim.FromResult(GetDevice(id, state) as IDevice);
        }

        IDevice IAudioController.GetDefaultDevice(DeviceType deviceType, Role role)
        {
            return GetDefaultDevice(deviceType, role);
        }

        Task<IDevice> IAudioController.GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return TaskShim.FromResult(GetDefaultDevice(deviceType, role) as IDevice);
        }

        IEnumerable<IDevice> IAudioController.GetDevices()
        {
            return GetDevices();
        }

        Task<IEnumerable<IDevice>> IAudioController.GetDevicesAsync()
        {
            return TaskShim.FromResult(GetDevices().Cast<IDevice>());
        }

        IEnumerable<IDevice> IAudioController.GetDevices(DeviceState state)
        {
            return GetDevices(state);
        }

        Task<IEnumerable<IDevice>> IAudioController.GetDevicesAsync(DeviceState state)
        {
            return TaskShim.FromResult(GetDevices(state).Cast<IDevice>());
        }

        IEnumerable<IDevice> IAudioController.GetDevices(DeviceType deviceType, DeviceState state)
        {
            return GetDevices(deviceType, state);
        }

        Task<IEnumerable<IDevice>> IAudioController.GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            return TaskShim.FromResult(GetDevices(deviceType, state).Cast<IDevice>());
        }

        IEnumerable<IDevice> IAudioController.GetPlaybackDevices()
        {
            return GetPlaybackDevices();
        }

        IEnumerable<IDevice> IAudioController.GetPlaybackDevices(DeviceState state)
        {
            return GetPlaybackDevices(state);
        }

        Task<IEnumerable<IDevice>> IAudioController.GetPlaybackDevicesAsync()
        {
            return TaskShim.FromResult(GetPlaybackDevices().Cast<IDevice>());
        }

        Task<IEnumerable<IDevice>> IAudioController.GetPlaybackDevicesAsync(DeviceState deviceState)
        {
            return TaskShim.FromResult(GetPlaybackDevices(deviceState).Cast<IDevice>());
        }

        IEnumerable<IDevice> IAudioController.GetCaptureDevices()
        {
            return GetCaptureDevices();
        }

        IEnumerable<IDevice> IAudioController.GetCaptureDevices(DeviceState state)
        {
            return GetCaptureDevices(state);
        }

        Task<IEnumerable<IDevice>> IAudioController.GetCaptureDevicesAsync()
        {
            return TaskShim.FromResult(GetCaptureDevices().OfType<IDevice>());
        }

        Task<IEnumerable<IDevice>> IAudioController.GetCaptureDevicesAsync(DeviceState deviceState)
        {
            return TaskShim.FromResult(GetCaptureDevices(deviceState).OfType<IDevice>());
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