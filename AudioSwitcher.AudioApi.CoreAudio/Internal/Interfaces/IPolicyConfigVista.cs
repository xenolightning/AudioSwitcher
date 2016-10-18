using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.POLICY_CONFIG_VISTA_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPolicyConfigVista
    {
        [PreserveSig]
        int GetMixFormat(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [Out] out WaveFormatExtensible ppFormat);

        [PreserveSig]
        int GetDeviceFormat(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] [MarshalAs(UnmanagedType.Bool)] bool bDefault,
            [Out] out WaveFormatExtensible ppFormat);

        [PreserveSig]
        int SetDeviceFormat(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] WaveFormatExtensible pEndpointFormat,
            [In] WaveFormatExtensible mixFormat);

        [PreserveSig]
        int GetProcessingPeriod(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] [MarshalAs(UnmanagedType.Bool)] bool bDefault,
            [In] IntPtr pmftDefaultPeriod,
            [In] IntPtr pmftMinimumPeriod);

        [PreserveSig]
        int SetProcessingPeriod(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] IntPtr pmftPeriod);

        [PreserveSig]
        int GetShareMode(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [Out] out DeviceShareMode pMode);

        [PreserveSig]
        int SetShareMode(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] DeviceShareMode mode);

        [PreserveSig]
        int GetPropertyValue(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] [MarshalAs(UnmanagedType.Bool)] bool bFxStore,
            [In] IntPtr key,
            [In] IntPtr pv);

        [PreserveSig]
        int SetPropertyValue(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] [MarshalAs(UnmanagedType.Bool)] bool bFxStore,
            [In] IntPtr key,
            [In] IntPtr pv);

        [PreserveSig]
        int SetDefaultEndpoint(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] [MarshalAs(UnmanagedType.U4)] ERole role);

        [PreserveSig]
        int SetEndpointVisibility(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
            [In] [MarshalAs(UnmanagedType.Bool)] bool bVisible);
    }
}