using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.DirectSound
{
    // Direct Sound Interface:
    // https://msdn.microsoft.com/en-us/library/ee418035%28v=vs.85%29.aspx
    //

    [Guid("C50A7E93-F395-4834-9EF6-7FA99DE50966")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDirectSound
    {
        [PreserveSig]
        int CreateSoundBuffer([In, Out] ref IntPtr pcDsBufferDesc, [Out] out IntPtr ppDsBuffer, [In] IntPtr pUnkOuter);

        [PreserveSig]
        int GetCaps([In, Out] ref IntPtr pDsCaps);

        [PreserveSig]
        int DuplicateSoundBuffer([In] IntPtr pDsBufferOriginal, [Out] out IntPtr ppDsBufferDuplicate);

        [PreserveSig]
        int SetCooperativeLevel([In] IntPtr hwnd, [In] uint dwLevel);

        [PreserveSig]
        int Compact();

        [PreserveSig]
        int GetSpeakerConfig([Out] out ESpeakerConfig pdwSpeakerConfig);

        [PreserveSig]
        int SetSpeakerConfig([In] ESpeakerConfig dwSpeakerConfig);

        [PreserveSig]
        int Initialize([In, Out] ref Guid pcGuidDevice);

        [PreserveSig]
        int VerifyCertification([Out] out uint pdwCertified);
    }
}