using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.AUDIO_CHANNEL_CONFIG_IID), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioChannelConfig
    {

        [PreserveSig]
        int SetChannelConfig([In] [MarshalAs(UnmanagedType.U4)] SpeakerConfiguration channelConfig, [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [PreserveSig]
        int GetChannelConfig([Out] [MarshalAs(UnmanagedType.U4)] out SpeakerConfiguration channelConfig);

    }
}
