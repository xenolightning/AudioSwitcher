using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.PARTS_LIST_IID), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPartsList
    {
        [PreserveSig]
        int GetCount([Out] [MarshalAs(UnmanagedType.U4)] out uint count);

        [PreserveSig]
        int GetPart([In]  [MarshalAs(UnmanagedType.U4)] uint index, [Out] [MarshalAs(UnmanagedType.Interface)] out IPart part);
    }
}
