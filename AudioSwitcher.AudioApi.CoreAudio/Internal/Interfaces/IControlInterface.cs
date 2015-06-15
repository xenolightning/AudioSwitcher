using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComIIds.CONTROL_INTERFACE_IID), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IControlInterface
    {
        [PreserveSig]
        int GetName([Out] [MarshalAs(UnmanagedType.LPWStr)] out string name);

        [PreserveSig]
        int GetIID([Out] out Guid interfaceId);
    }
}
