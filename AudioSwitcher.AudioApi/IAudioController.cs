using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IAudioController : IDisposable
    {
        IDevice DefaultPlaybackDevice { get; set; }

        IDevice DefaultPlaybackCommunicationsDevice { get; set; }

        IDevice DefaultCaptureDevice { get; set; }

        IDevice DefaultCaptureCommunicationsDevice { get; set; }

        event AudioDeviceChangedHandler AudioDeviceChanged;

        IEnumerable<IDevice> GetAllDevices();

        IEnumerable<IDevice> GetAllDevices(DeviceState deviceState);

        Task<IEnumerable<IDevice>> GetAllDevicesAsync();

        Task<IEnumerable<IDevice>> GetAllDevicesAsync(DeviceState deviceState);

        IEnumerable<IDevice> GetPlaybackDevices();

        IEnumerable<IDevice> GetPlaybackDevices(DeviceState deviceState);

        Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync();

        Task<IEnumerable<IDevice>> GetPlaybackDevicesAsync(DeviceState deviceState);

        IEnumerable<IDevice> GetCaptureDevices();

        IEnumerable<IDevice> GetCaptureDevices(DeviceState deviceState);

        Task<IEnumerable<IDevice>> GetCaptureDevicesAsync();

        Task<IEnumerable<IDevice>> GetCaptureDevicesAsync(DeviceState deviceState);

        IDevice GetAudioDevice(Guid id);

        IDevice GetAudioDevice(Guid id, DeviceState state);

        Task<IDevice> GetAudioDeviceAsync(Guid id);

        Task<IDevice> GetAudioDeviceAsync(Guid id, DeviceState state);

        bool SetDefaultDevice(IDevice dev);

        bool SetDefaultCommunicationsDevice(IDevice dev);

        Task<bool> SetDefaultDeviceAsync(IDevice dev);

        Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev);
    }
}