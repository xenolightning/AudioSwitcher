using System;
using NAudio.CoreAudioApi;

namespace AudioSwitcher.AudioApi.NAudio
{
    public static class MMDeviceExtensions
    {
        public static readonly PropertyKey PKEY_Device_FriendlyName =
            new PropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 14);

        public static readonly PropertyKey PKEY_Device_Description =
            new PropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 2);

        public static readonly PropertyKey PKEY_Device_Icon =
            new PropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x08, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 12);

        public static readonly PropertyKey PKEY_System_Name =
            new PropertyKey(new Guid(0xb3f8fa53, 0x0004, 0x438e, 0x90, 0x03, 0x51, 0xa4, 0x6e, 0x13, 0x9b, 0xfc), 6);

        /// <summary>
        ///     Friendly name of device
        /// </summary>
        public static string GetSystemName(this MMDevice device)
        {
            if (device.Properties.Contains(PKEY_System_Name))
            {
                return (string) device.Properties[PKEY_System_Name].Value;
            }
            return "Unknown";
        }

        /// <summary>
        ///     Fully qualified name of the device
        /// </summary>
        public static string GetFullName(this MMDevice device)
        {
            try
            {
                if (device.Properties.Contains(PropertyKeys.PKEY_Device_FriendlyName) &&
                    device.Properties.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
                {
                    return device.Properties[PropertyKeys.PKEY_Device_FriendlyName].Value + " (" +
                           device.Properties[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value + ")";
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        ///     Friendly name of device
        /// </summary>
        public static string GetDeviceFriendlyName(this MMDevice device)
        {
            if (device.Properties.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
            {
                return (string) device.Properties[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
            }
            return "Unknown";
        }

        /// <summary>
        ///     The name of the device
        /// </summary>
        public static string GetDeviceName(this MMDevice device)
        {
            if (device.Properties.Contains(PKEY_Device_Description))
            {
                return (string) device.Properties[PKEY_Device_Description].Value;
            }
            return "Unknown";
        }

        /// <summary>
        ///     The Icon of the device
        /// </summary>
        public static string GetIconPath(this MMDevice device)
        {
            try
            {
                if (device.Properties.Contains(PKEY_Device_Icon))
                {
                    return (string) device.Properties[PKEY_Device_Icon].Value;
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        ///     The name of the device
        /// </summary>
        public static string GetFriendlyName(this MMDevice device)
        {
            if (device.Properties.Contains(PKEY_Device_FriendlyName))
            {
                return (string) device.Properties[PKEY_Device_FriendlyName].Value;
            }
            return "Unknown";
        }
    }
}