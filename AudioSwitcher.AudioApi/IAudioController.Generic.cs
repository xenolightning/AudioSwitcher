using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IAudioController<T> : IAudioController
        where T : IDevice
    {
        new T DefaultPlaybackDevice { get; }

        new T DefaultPlaybackCommunicationsDevice { get; }

        new T DefaultCaptureDevice { get; }

        new T DefaultCaptureCommunicationsDevice { get; }

        new T GetDevice(Guid id);

        new Task<T> GetDeviceAsync(Guid id);

        new T GetDevice(Guid id, DeviceState state);

        new Task<T> GetDeviceAsync(Guid id, DeviceState state);

        new T GetDefaultDevice(DeviceType deviceType, Role role);

        new Task<T> GetDefaultDeviceAsync(DeviceType deviceType, Role role);

        new IEnumerable<T> GetDevices();

        new Task<IEnumerable<T>> GetDevicesAsync();

        new IEnumerable<T> GetDevices(DeviceState state);

        new Task<IEnumerable<T>> GetDevicesAsync(DeviceState state);

        new IEnumerable<T> GetDevices(DeviceType deviceType);

        new Task<IEnumerable<T>> GetDevicesAsync(DeviceType deviceType);

        new IEnumerable<T> GetDevices(DeviceType deviceType, DeviceState state);

        new Task<IEnumerable<T>> GetDevicesAsync(DeviceType deviceType, DeviceState state);

        new IEnumerable<T> GetPlaybackDevices();

        new IEnumerable<T> GetPlaybackDevices(DeviceState state);

        new Task<IEnumerable<T>> GetPlaybackDevicesAsync();

        new Task<IEnumerable<T>> GetPlaybackDevicesAsync(DeviceState deviceState);

        new IEnumerable<T> GetCaptureDevices();

        new IEnumerable<T> GetCaptureDevices(DeviceState state);

        new Task<IEnumerable<T>> GetCaptureDevicesAsync();

        new Task<IEnumerable<T>> GetCaptureDevicesAsync(DeviceState deviceState);
    }
}