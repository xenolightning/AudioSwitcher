using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IAudioController<T> : IAudioController
        where T : IDevice
    {
        new T DefaultPlaybackDevice { get; set; }

        new T DefaultPlaybackCommunicationsDevice { get; set; }

        new T DefaultCaptureDevice { get; set; }

        new T DefaultCaptureCommunicationsDevice { get; set; }

        new IEnumerable<T> GetAllDevices();

        new IEnumerable<T> GetAllDevices(DeviceState deviceState);

        new Task<IEnumerable<T>> GetAllDevicesAsync();

        new Task<IEnumerable<T>> GetAllDevicesAsync(DeviceState deviceState);

        new IEnumerable<T> GetPlaybackDevices();

        new IEnumerable<T> GetPlaybackDevices(DeviceState deviceState);

        new Task<IEnumerable<T>> GetPlaybackDevicesAsync();

        new Task<IEnumerable<T>> GetPlaybackDevicesAsync(DeviceState deviceState);

        new IEnumerable<T> GetCaptureDevices();

        new IEnumerable<T> GetCaptureDevices(DeviceState deviceState);

        new Task<IEnumerable<T>> GetCaptureDevicesAsync();

        new Task<IEnumerable<T>> GetCaptureDevicesAsync(DeviceState deviceState);

        new T GetAudioDevice(Guid id);

        new T GetAudioDevice(Guid id, DeviceState state);

        new Task<T> GetAudioDeviceAsync(Guid id);

        new Task<T> GetAudioDeviceAsync(Guid id, DeviceState state);

        bool SetDefaultDevice(T dev);

        Task<bool> SetDefaultDeviceAsync(T dev);

        bool SetDefaultCommunicationsDevice(T dev);

        Task<bool> SetDefaultCommunicationsDeviceAsync(T dev);
    }
}