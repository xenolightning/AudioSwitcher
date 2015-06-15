using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IMMDeviceEnumerator
    {
        [PreserveSig]
        int EnumAudioEndpoints(EDataFlow eDataFlow, EDeviceState stateMask, out IntPtr device);

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow eDataFlow, ERole eRole, out IntPtr ppEndpoint);

        [PreserveSig]
        int GetDevice(string pwstrId, out IntPtr ppDevice);

        [PreserveSig]
        int RegisterEndpointNotificationCallback(IntPtr pClient);

        [PreserveSig]
        int UnregisterEndpointNotificationCallback(IntPtr pClient);
    }
}