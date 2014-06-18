using System.Collections.Generic;

namespace AudioSwitcher.AudioApi.Plugins
{
    public interface IPreferredDeviceManager : IControllerPlugin
    {
        IList<IDevice> PreferredDevices { get; }

        IDevice NextPlaybackDevice();

        IDevice PreviousPlaybackDevice();

        IDevice NextCaptureDevice();

        IDevice PreviousCaptureDevice();

        void AddDevice(IDevice ad, int position = 0);

        void RemoveDevice(IDevice ad);

        bool IsPreferredDevice(IDevice ad);

        byte[] Save();

        void Load(byte[] bytes);
    }
}
