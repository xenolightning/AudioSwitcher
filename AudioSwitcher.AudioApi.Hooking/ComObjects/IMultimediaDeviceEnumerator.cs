using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Guid(ComIIds.IMM_DEVICE_ENUMERATOR_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IMultimediaDeviceEnumerator
    {
        [PreserveSig]
        int EnumAudioEndpoints(DataFlow dataFlow, DeviceState stateMask, out IntPtr device);

        [PreserveSig]
        int GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IntPtr ppEndpoint);

        [PreserveSig]
        int GetDevice(string pwstrId, out IntPtr ppDevice);

        [PreserveSig]
        int RegisterEndpointNotificationCallback(IntPtr pClient);

        [PreserveSig]
        int UnregisterEndpointNotificationCallback(IntPtr pClient);
    }
}