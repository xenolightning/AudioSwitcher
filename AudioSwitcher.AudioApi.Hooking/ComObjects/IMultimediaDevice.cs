using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Guid(ComIIds.IMM_DEVICE_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMultimediaDevice
    {
        [PreserveSig]
        int Activate(
            [In] ref Guid interfaceId,
            [In] [MarshalAs(UnmanagedType.U4)] IntPtr classContext,
            [In, Optional] IntPtr activationParams,
            [Out] [MarshalAs(UnmanagedType.IUnknown)] out object instancePtr);

        [PreserveSig]
        int OpenPropertyStore(
            [In] [MarshalAs(UnmanagedType.U4)] IntPtr accessMode,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IntPtr properties);

        [PreserveSig]
        int GetId([Out] [MarshalAs(UnmanagedType.LPWStr)] out string strId);

        [PreserveSig]
        int GetState([Out] [MarshalAs(UnmanagedType.U4)] out DeviceState deviceState);
    }
}