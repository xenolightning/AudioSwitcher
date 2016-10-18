using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.AUDIO_VOLUME_DUCK_NOTIFICATION_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioVolumeDuckNotification
    {
        [PreserveSig]
        int OnVolumeDuckNotification(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string sessionId,
            [In] uint activeSessionCount);

        [PreserveSig]
        int OnVolumeUnduckNotification([In] [MarshalAs(UnmanagedType.LPWStr)] string sessionId);
    }
}