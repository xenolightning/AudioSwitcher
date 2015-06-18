using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.DirectSound;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class NativeMethods
    {
        [DllImport("ole32.dll")]
        public static extern int PropVariantClear(ref PropVariant pvar);

        [DllImport("dsound.dll", PreserveSig = true, EntryPoint = "DirectSoundCreate8")]
        public static extern int DirectSoundCreate8([In, Out] ref Guid lpcGuidDevice, [Out] out IDirectSound directSound, [In] IntPtr pUnkOuter = default(IntPtr));
    }
}