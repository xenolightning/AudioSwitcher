using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.IMM_DEVICE_ENUMERATOR_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMultimediaDeviceEnumerator
    {
        [PreserveSig]
        int EnumAudioEndpoints(
            [In] [MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow,
            [In] [MarshalAs(UnmanagedType.U4)] EDeviceState stateMask,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IMultimediaDeviceCollection devices);

        [PreserveSig]
        int GetDefaultAudioEndpoint(
            [In] [MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow,
            [In] [MarshalAs(UnmanagedType.I4)] ERole role,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IMultimediaDevice device);

        [PreserveSig]
        int GetDevice(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string endpointId,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IMultimediaDevice device);

        [PreserveSig]
        int RegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMultimediaNotificationClient client);

        [PreserveSig]
        int UnregisterEndpointNotificationCallback(
            [In] [MarshalAs(UnmanagedType.Interface)] IMultimediaNotificationClient client);
    }
}