using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.AUDIO_ENDPOINT_VOLUME_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioEndpointVolume
    {
        [PreserveSig]
        int RegisterControlChangeNotify([In] [MarshalAs(UnmanagedType.Interface)] IAudioEndpointVolumeCallback client);

        [PreserveSig]
        int UnregisterControlChangeNotify([In] [MarshalAs(UnmanagedType.Interface)] IAudioEndpointVolumeCallback client);

        [PreserveSig]
        int GetChannelCount([Out] [MarshalAs(UnmanagedType.U4)] out int channelCount);

        [PreserveSig]
        int SetMasterVolumeLevel(
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int SetMasterVolumeLevelScalar(
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int GetMasterVolumeLevel(
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        [PreserveSig]
        int GetMasterVolumeLevelScalar(
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        [PreserveSig]
        int SetChannelVolumeLevel(
            [In] [MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int SetChannelVolumeLevelScalar(
            [In] [MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int GetChannelVolumeLevel(
            [In] [MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        [PreserveSig]
        int GetChannelVolumeLevelScalar(
            [In] [MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        [PreserveSig]
        int SetMute(
            [In] [MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int GetMute(
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool isMuted);

        [PreserveSig]
        int GetVolumeStepInfo(
            [Out] [MarshalAs(UnmanagedType.U4)] out uint step,
            [Out] [MarshalAs(UnmanagedType.U4)] out uint stepCount);

        [PreserveSig]
        int VolumeStepUp(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int VolumeStepDown(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int QueryHardwareSupport(
            [Out] [MarshalAs(UnmanagedType.U4)] out uint hardwareSupportMask);

        [PreserveSig]
        int GetVolumeRange(
            [Out] [MarshalAs(UnmanagedType.R4)] out float volumeMin,
            [Out] [MarshalAs(UnmanagedType.R4)] out float volumeMax,
            [Out] [MarshalAs(UnmanagedType.R4)] out float volumeStep);
    }
}