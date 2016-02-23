using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.AUDIO_SESSION_MANAGER2_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager2
    {
        [PreserveSig]
        int GetAudioSessionControl(
            [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid sessionId,
            [In] [MarshalAs(UnmanagedType.U4)] uint streamFlags,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl sessionControl);

        [PreserveSig]
        int GetSimpleAudioVolume(
            [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid sessionId,
            [In] [MarshalAs(UnmanagedType.U4)] uint streamFlags,
            [Out] [MarshalAs(UnmanagedType.Interface)] out ISimpleAudioVolume audioVolume);

        [PreserveSig]
        int GetSessionEnumerator(
            [Out] [MarshalAs(UnmanagedType.Interface)] out IAudioSessionEnumerator sessionList);

        [PreserveSig]
        int RegisterSessionNotification([In] IAudioSessionNotification client);

        [PreserveSig]
        int UnregisterSessionNotification([In] IAudioSessionNotification client);

        [PreserveSig]
        int RegisterDuckNotification(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string sessionId,
            [In] IAudioVolumeDuckNotification client);

        [PreserveSig]
        int UnregisterDuckNotification([In] IAudioVolumeDuckNotification client);
    }
}