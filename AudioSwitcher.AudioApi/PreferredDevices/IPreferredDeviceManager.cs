using System.Collections.Generic;

namespace AudioSwitcher.AudioApi.PreferredDevices
{
    public interface IPreferredDeviceManager : IControllerPlugin
    {
        IEnumerable<IDevice> PreferredDevices { get; }

        IDevice NextPlaybackDevice();

        IDevice PreviousPlaybackDevice();

        IDevice NextRecordingDevice();

        IDevice PreviousRecordingDevice();

        void AddDevice(IDevice ad, int position = 0);

        void RemoveDevice(IDevice ad);

        bool IsPreferredDevice(IDevice ad);

        byte[] Save();

        void Load(byte[] bytes);
    }
}
