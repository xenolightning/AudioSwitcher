using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
    internal struct WAVEFORMATEX
    {
        private ushort cbSize; /* the count in bytes of the size of */
        private uint nAvgBytesPerSec; /* for buffer estimation */
        private ushort nBlockAlign; /* block size of data */
        private ushort nChannels; /* number of channels (i.e. mono, stereo...) */
        private uint nSamplesPerSec; /* sample rate */
        private ushort wBitsPerSample; /* number of bits per sample of mono data */
        private ushort wFormatTag; /* format type */
        /* extra information (after cbSize) */
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
        int SetDefaultEndpoint(string pszDeviceName, Role role);
    }
}