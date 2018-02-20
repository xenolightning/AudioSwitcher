using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.AUDIO_CLIENT_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioClient
    {
        [PreserveSig]
        int Initialize(DeviceShareMode shareMode,
            [In] [MarshalAs(UnmanagedType.U4)] uint streamFlags,
            [In] [MarshalAs(UnmanagedType.I8)] long hnsBufferDuration,
            [In] [MarshalAs(UnmanagedType.I8)] long hnsPeriodicity,
            [In] WaveFormatEx pFormat,
            [In, Out] [MarshalAs(UnmanagedType.LPStruct)] ref Guid audioSessionGuid);

        [PreserveSig]
        int GetBufferSize([Out] [MarshalAs(UnmanagedType.U4)] out uint bufferSize);

        [PreserveSig]
        int GetStreamLatency([Out] [MarshalAs(UnmanagedType.I8)] out long bufferSize);

        [PreserveSig]
        int GetCurrentPadding(out int currentPadding);

        [PreserveSig]
        int IsFormatSupported(
            DeviceShareMode shareMode,
            [In] WaveFormatEx pFormat,
            [Out] out WaveFormatEx closestMatchFormat);

        [PreserveSig]
        int GetMixFormat([Out] out WaveFormatEx format);

        [PreserveSig]
        int GetDevicePeriod(
            [Out] [MarshalAs(UnmanagedType.I8)] out long defaultDevicePeriod, 
            [Out] [MarshalAs(UnmanagedType.I8)]out long minimumDevicePeriod);

        [PreserveSig]
        int Start();

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int SetEventHandle(IntPtr eventHandle);

        [PreserveSig]
        int GetService(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId, 
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
    }
}
