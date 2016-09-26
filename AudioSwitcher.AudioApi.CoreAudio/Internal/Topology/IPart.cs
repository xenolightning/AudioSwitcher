using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio.Topology
{
    [Guid(ComInterfaceIds.PART_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPart
    {
        [PreserveSig]
        int GetName([Out] [MarshalAs(UnmanagedType.LPWStr)] out string name);

        [PreserveSig]
        int GetLocalId([Out] [MarshalAs(UnmanagedType.U4)] out uint localId);

        [PreserveSig]
        int GetGlobalId([Out] [MarshalAs(UnmanagedType.LPWStr)] out string globalId);

        [PreserveSig]
        int GetPartType([Out] out PartType partType);

        [PreserveSig]
        int GetSubType([Out] out Guid subType);

        [PreserveSig]
        int GetControlInterfaceCount([Out] [MarshalAs(UnmanagedType.U4)] out uint count);

        [PreserveSig]
        int GetControlInterface([In] [MarshalAs(UnmanagedType.U4)] uint index, [Out] [MarshalAs(UnmanagedType.Interface)] out IControlInterface control);

        [PreserveSig]
        int EnumPartsIncoming([Out] [MarshalAs(UnmanagedType.Interface)] out IPartsList partList);

        [PreserveSig]
        int EnumPartsOutgoing([Out] [MarshalAs(UnmanagedType.Interface)] out IPartsList partList);

        [PreserveSig]
        int GetTopologyObject([Out] [MarshalAs(UnmanagedType.Interface)] out IDeviceTopology deviceTopology);

        [PreserveSig]
        int Activate([In] ClassContext classContext, [In] ref Guid interfaceId, [Out] [MarshalAs(UnmanagedType.IUnknown)] out object instancePtr);

        [PreserveSig]
        int RegisterControlChangeCallback([In] ref Guid interfaceId, [In] IControlChangeNotify client);

        [PreserveSig]
        int UnregisterControlChangeCallback([In] IControlChangeNotify client);
    }
}
