using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComIIds.AUDIO_METER_INFORMATION_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioMeterInformation
    {
        [PreserveSig]
        int GetPeakValue([Out] [MarshalAs(UnmanagedType.R4)] out float peak);

        [PreserveSig]
        int GetMeteringChannelCount([Out] [MarshalAs(UnmanagedType.U4)] out int channelCount);

        [PreserveSig]
        int GetChannelsPeakValues(
            [In] [MarshalAs(UnmanagedType.U4)] int channelCount,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)] float[] peakValues);

        [PreserveSig]
        int QueryHardwareSupport([Out] [MarshalAs(UnmanagedType.U4)] out UInt32 hardwareSupportMask);
    }
}
