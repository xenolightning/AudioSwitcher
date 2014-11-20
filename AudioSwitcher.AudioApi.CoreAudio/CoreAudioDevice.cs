using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [ComVisible(false)]
    public sealed class CoreAudioDevice : Device, INotifyPropertyChanged, IDisposable
    {
        private MMDevice _device;
        private Guid? _id;

        internal CoreAudioDevice(MMDevice device, IDeviceEnumerator<CoreAudioDevice> enumerator)
            : base(enumerator)
        {
            if (device == null)
                throw new ArgumentNullException("device", "Device cannot be null. Something bad went wrong");

            Device = device;

            //Memory leak here, for some reason subscribing to this event is preventing a recycle

            if (Device.AudioEndpointVolume != null)
                Device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            //enumerator.AudioDeviceChanged += EnumeratorOnAudioDeviceChanged;
        }

        ~CoreAudioDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Device != null)
            {
                Device.Dispose();
            }

            GC.SuppressFinalize(this);
        }


        /// <summary>
        ///     Accesssor to lower level device
        /// </summary>
        /// <remarks>Has to remain private to correctly control access to the ComObject in non STA threads</remarks>
        private MMDevice Device
        {
            get
            {
                return _device;
            }
            set
            {
                _device = value;
            }
        }

        /// <summary>
        ///     Unique identifier for this device
        /// </summary>
        public override Guid Id
        {
            get
            {
                if (_id == null)
                    _id = SystemIdToGuid(RealId);

                return _id.Value;
            }
        }

        public string RealId
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
                return (Enumerator.DefaultPlaybackDevice != null && Enumerator.DefaultPlaybackDevice.Id == Id)
                       || (Enumerator.DefaultCaptureDevice != null && Enumerator.DefaultCaptureDevice.Id == Id);
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                return (Enumerator.DefaultCommunicationsPlaybackDevice != null && Enumerator.DefaultCommunicationsPlaybackDevice.Id == Id)
                       || (Enumerator.DefaultCommunicationsCaptureDevice != null && Enumerator.DefaultCommunicationsCaptureDevice.Id == Id);
            }
        }

        public override DeviceState State
        {
            get
            {
                return Device.State.AsDeviceState();
            }
        }

        public override DeviceType DeviceType
        {
            get { return Device.EDataFlow.AsDeviceType(); }
        }

        public override bool IsMuted
        {
            get
            {
                if (Device.AudioEndpointVolume == null)
                    return false;

                return Device.AudioEndpointVolume.Mute;
            }
        }

        /// <summary>
        ///     The volume level on a scale between 0-100. Returns -1 if end point does not have volume
        /// </summary>
        public override int Volume
        {
            get
            {
                if (Device.AudioEndpointVolume == null)
                    return -1;

                return (int)Math.Round(Device.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0);
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;

                float val = (float)value / 100;

                if (Device.AudioEndpointVolume == null)
                    return;

                Device.AudioEndpointVolume.MasterVolumeLevelScalar = val;

                //Something is up with the floating point numbers in Windows, so make sure the volume is correct
                if (Device.AudioEndpointVolume.MasterVolumeLevelScalar < val)
                    Device.AudioEndpointVolume.MasterVolumeLevelScalar += 0.0001F;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void EnumeratorOnAudioDeviceChanged(object sender,
            AudioDeviceChangedEventArgs audioDeviceChangedEventArgs)
        {
            if (audioDeviceChangedEventArgs.Device.Id != Id)
                return;

            if (audioDeviceChangedEventArgs.EventType == AudioDeviceEventType.PropertyChanged)
            {
                OnPropertyChanged("DeviceType");
                OnPropertyChanged("Description");
                OnPropertyChanged("FullName");
                OnPropertyChanged("IconPath");
                OnPropertyChanged("Id");
                OnPropertyChanged("IsCaptureDevice");
                OnPropertyChanged("IsDefaultCommunicationsDevice");
                OnPropertyChanged("IsDefaultDevice");
                OnPropertyChanged("IsMuted");
                OnPropertyChanged("IsPlaybackDevice");
                OnPropertyChanged("ShortName");
                OnPropertyChanged("State");
                OnPropertyChanged("SystemName");
            }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            RaiseVolumeChanged();
        }

        private void RaiseVolumeChanged()
        {
            var handler = VolumeChanged;

            if (handler != null)
                handler(this, new AudioDeviceChangedEventArgs(this, AudioDeviceEventType.Volume));

            OnPropertyChanged("Volume");
        }

        /// <summary>
        ///     Mute this device
        /// </summary>
        public override bool Mute()
        {
            if (Device.AudioEndpointVolume == null)
                return false;

            Device.AudioEndpointVolume.Mute = true;
            return Device.AudioEndpointVolume.Mute;
        }

        /// <summary>
        ///     Unmute this device
        /// </summary>
        public override bool UnMute()
        {
            if (Device.AudioEndpointVolume == null)
                return false;

            Device.AudioEndpointVolume.Mute = false;
            return Device.AudioEndpointVolume.Mute;
        }

        public override event AudioDeviceChangedHandler VolumeChanged;

        /// <summary>
        ///     Extracts the unique GUID Identifier for a Windows System Device
        /// </summary>
        /// <param name="systemDeviceId"></param>
        /// <returns></returns>
        public static Guid SystemIdToGuid(string systemDeviceId)
        {
            string[] dev = systemDeviceId.Replace("{", "")
                .Replace("}", "")
                .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return new Guid(dev[dev.Length - 1]);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}