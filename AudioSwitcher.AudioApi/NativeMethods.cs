using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx([MarshalAs(UnmanagedType.LPWStr)] string szFileName, int nIconIndex,
            IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);
    }
}