using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int EnumAudioEndpoints(EDataFlow eDataFlow, EDeviceState stateMask,
            out IMMDeviceCollection devices);

        int GetDefaultAudioEndpoint(EDataFlow eDataFlow, ERole role, out IMMDevice endpoint);

        int GetDevice(string id, out IMMDevice deviceName);

        int RegisterEndpointNotificationCallback(IMMNotificationClient client);

        int UnregisterEndpointNotificationCallback(IMMNotificationClient client);
    }
}