using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.System
{
    [ComVisible(false)]
    public sealed class SystemAudioDevice : AudioDevice
    {
        private Guid? _id;

        internal SystemAudioDevice(MMDevice device, IDeviceEnumerator<SystemAudioDevice> enumerator)
            : base(enumerator)
        {
            if (device == null)
                throw new ArgumentNullException("device", "Device cannot be null. Something bad went wrong");

            Device = device;
        }

        /// <summary>
        ///     Accesssor to lower level device
        /// </summary>
        internal MMDevice Device { get; private set; }

        /// <summary>
        ///     Unique identifier for this device
        /// </summary>
        public override Guid ID
        {
            get
            {
                if (_id == null)
                    _id = SystemIDToGuid(Device.ID);

                return _id.Value;
            }
        }

        public override string Description
        {
            get
            {
                if (Device == null)
                    return "Unknown";
                return Device.DeviceFriendlyName;
            }
        }

        public override string ShortName
        {
            get
            {
                if (Device == null)
                    return "Unknown";
                return Device.DeviceName;
            }
            set
            {
                Device.DeviceName = value;
            }
        }

        public override string SystemName
        {
            get
            {
                if (Device == null)
                    return "Unknown";
                return Device.SystemName;
            }
        }

        public override string FullName
        {
            get
            {
                if (Device != null)
                    return Device.DeviceFriendlyName;
                return "Unknown Device";
            }
        }

        public override string IconPath
        {
            get
            {
                if (Device != null)
                    return Device.IconPath;
                return "Unknown";
            }
        }

        public override bool IsDefaultDevice
        {
            get
            {
                return Enumerator.DefaultPlaybackDevice.ID == ID
                       || Enumerator.DefaultRecordingDevice.ID == ID;
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                return Enumerator.DefaultCommunicationsPlaybackDevice.ID == ID
                       || Enumerator.DefaultCommunicationsRecordingDevice.ID == ID;
            }
        }

        public override DeviceState State
        {
            get { return Device.State; }
        }

        public override DataFlow DataFlow
        {
            get { return Device.DataFlow; }
        }

        public override bool IsMuted
        {
            get { return Device.AudioEndpointVolume.Mute; }
        }

        /// <summary>
        ///     The volume level on a scale between 0-100. Returns -1 if end point does not have volume
        /// </summary>
        public override int Volume
        {
            get
            {
                try
                {
                    return (int)Math.Round(Device.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;

                float val = (float)value / 100;

                Device.AudioEndpointVolume.MasterVolumeLevelScalar = val;

                //Something is up with the floating point numbers in Windows, so make sure the volume is correct
                if (Device.AudioEndpointVolume.MasterVolumeLevelScalar < val)
                    Device.AudioEndpointVolume.MasterVolumeLevelScalar += 0.0001F;
            }
        }

        /// <summary>
        ///     Mute this device
        /// </summary>
        public override bool Mute()
        {
            Device.AudioEndpointVolume.Mute = true;
            return Device.AudioEndpointVolume.Mute;
        }

        /// <summary>
        ///     Unmute this device
        /// </summary>
        public override bool UnMute()
        {
            Device.AudioEndpointVolume.Mute = false;
            return Device.AudioEndpointVolume.Mute;
        }

        /// <summary>
        ///     Extracts the unique GUID Identifier for a Windows System Device
        /// </summary>
        /// <param name="systemDeviceId"></param>
        /// <returns></returns>
        public static Guid SystemIDToGuid(string systemDeviceId)
        {
            string[] dev = systemDeviceId.Replace("{", "")
                .Replace("}", "")
                .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return new Guid(dev[dev.Length - 1]);
        }
    }
}