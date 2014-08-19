using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    // ReSharper disable once InconsistentNaming
    [StructLayout(LayoutKind.Sequential)]
    internal struct WAVEFORMATEX
    {
        private readonly ushort wFormatTag;
        private readonly ushort nChannels;
        private readonly uint nSamplesPerSec;
        private readonly uint nAvgBytesPerSec;
        private readonly ushort nBlockAlign;
        private readonly ushort wBitsPerSample;
        private readonly ushort cbSize;
    }

    internal enum DeviceShareMode
    {
        DeviceShared,
        DeviceExclusive
    }

    [Guid("568b9108-44bf-40b4-9006-86afe5b5a620"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPolicyConfigVista
    {
        [PreserveSig]
        int GetMixFormat(string pszDeviceName, WAVEFORMATEX waveFormat);

        // not available on Windows 7, use method from IPolicyConfig
        [PreserveSig]
        int GetDeviceFormat(string pszDeviceName, int bDefault, WAVEFORMATEX ppFormat);

        [PreserveSig]
        int SetDeviceFormat(string pszDeviceName, WAVEFORMATEX pEndpointFormat, WAVEFORMATEX MixFormat);

        [PreserveSig]
        int GetProcessingPeriod(string pszDeviceName, int input2, long long1, long long2);

        // not available on Windows 7, use method from IPolicyConfig
        [PreserveSig]
        int SetProcessingPeriod(string pszDeviceName, long long1);

        // not available on Windows 7, use method from IPolicyConfig
        [PreserveSig]
        int GetShareMode(string pszDeviceName, DeviceShareMode devShareMode);

        // not available on Windows 7, use method from IPolicyConfig
        [PreserveSig]
        int SetShareMode(string pszDeviceName, DeviceShareMode shareMode);

        // not available on Windows 7, use method from IPolicyConfig
        [PreserveSig]
        int GetPropertyValue(string pszDeviceName, out PropertyKey propKey, out PropVariant propVariant);

        [PreserveSig]
        int SetPropertyValue(string pszDeviceName, PropertyKey propKey, PropVariant propVariant);

        [PreserveSig]
        int SetDefaultEndpoint(string pszDeviceName, ERole role);
    }
}