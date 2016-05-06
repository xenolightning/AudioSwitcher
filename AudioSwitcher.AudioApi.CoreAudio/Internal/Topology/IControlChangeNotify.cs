using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio.Topology
{
    [Guid(ComInterfaceIds.CONTROL_CHANGE_NOTIFY_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IControlChangeNotify
    {
        [PreserveSig]
        int OnNotify([In] [MarshalAs(UnmanagedType.U4)] uint processId, [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
    }
}
