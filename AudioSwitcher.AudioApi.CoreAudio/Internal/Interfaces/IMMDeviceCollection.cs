using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.IMM_DEVICE_COLLECTION_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceCollection
    {
        [PreserveSig]
        int GetCount([Out] [MarshalAs(UnmanagedType.U4)] out uint count);

        [PreserveSig]
        int Item(
            [In] [MarshalAs(UnmanagedType.U4)] uint index,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IMMDevice device);
    }
}
