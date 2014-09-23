using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public interface IDeviceEnumerator<T> : IDeviceEnumerator
        where T : IDevice
    {
        new T DefaultPlaybackDevice { get; }

        new T DefaultCommunicationsPlaybackDevice { get; }

        new T DefaultCaptureDevice { get; }

        new T DefaultCommunicationsCaptureDevice { get; }

        new T GetDevice(Guid id);

        new Task<T> GetDeviceAsync(Guid id);

        new T GetDefaultDevice(DeviceType deviceType, Role role);

        new Task<T> GetDefaultDeviceAsync(DeviceType deviceType, Role role);

        new IEnumerable<T> GetDevices(DeviceType deviceType, DeviceState state);

        new Task<IEnumerable<T>> GetDevicesAsync(DeviceType deviceType, DeviceState state);

        bool SetDefaultDevice(T dev);

        Task<bool> SetDefaultDeviceAsync(T dev);

        bool SetDefaultCommunicationsDevice(T dev);

        Task<bool> SetDefaultCommunicationsDeviceAsync(T dev);
    }
}