using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.SIMPLE_AUDIO_VOLUME_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISimpleAudioVolume
    {
        [PreserveSig]
        int SetMasterVolume(
            [In] [MarshalAs(UnmanagedType.R4)] float levelNorm,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int GetMasterVolume([Out] [MarshalAs(UnmanagedType.R4)] out float levelNorm);

        [PreserveSig]
        int SetMute(
            [In] [MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int GetMute([Out] [MarshalAs(UnmanagedType.Bool)] out bool isMuted);
    }
}