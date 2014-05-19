using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [ComVisible(false)]
    public sealed class CoreAudioDevice : Device, INotifyPropertyChanged
    {
        private Guid? _id;

        internal CoreAudioDevice(MMDevice device, IDeviceEnumerator<CoreAudioDevice> enumerator)
            : base(enumerator)
        {
            if (device == null)
                throw new ArgumentNullException("device", "Device cannot be null. Something bad went wrong");

            Device = device;
            if (Device.AudioEndpointVolume != null)
                Device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            RaiseVolumeChanged();
        }

        private void RaiseVolumeChanged()
        {
            if (VolumeChanged != null)
                VolumeChanged(this, new AudioDeviceChangedEventArgs(this, AudioDeviceEventType.Volume));

            OnPropertyChanged("Volume");
        }

        /// <summary>
        ///     Accesssor to lower level device
        /// </summary>
        internal MMDevice Device { get; private set; }

        /// <summary>
        ///     Unique identifier for this device
        /// </summary>
        public override Guid Id
        {
            get
            {
                if (_id == null)
                    _id = SystemIDToGuid(Device.ID);

                return _id.Value;
            }
        }

        public override string RealId
        {
            get
            {
                if (Device == null)
                    return String.Empty;

                return Device.ID;
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
            set { Device.DeviceName = value; }
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
                return Enumerator.DefaultPlaybackDevice.Id == Id
                       || Enumerator.DefaultRecordingDevice.Id == Id;
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                return Enumerator.DefaultCommunicationsPlaybackDevice.Id == Id
                       || Enumerator.DefaultCommunicationsRecordingDevice.Id == Id;
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

        public override event AudioDeviceChangedHandler VolumeChanged;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}