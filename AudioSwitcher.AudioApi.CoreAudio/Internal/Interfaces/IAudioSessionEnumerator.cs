using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComInterfaceIds.AUDIO_SESSION_ENUMERATOR_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEnumerator
    {
        [PreserveSig]
        int GetCount([Out] [MarshalAs(UnmanagedType.I4)] out int count);

        [PreserveSig]
        int GetSession(
            [In] [MarshalAs(UnmanagedType.I4)] int index,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl session);
    }
}