using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [ComVisible(false)]
    public sealed class CoreAudioDevice : Device, INotifyPropertyChanged, IDisposable
    {
        private readonly MMDevice _device;
        private Guid? _id;

        internal CoreAudioDevice(MMDevice device, IDeviceEnumerator<CoreAudioDevice> enumerator)
            : base(enumerator)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            _device = device;

            if (_device.AudioEndpointVolume != null)
                _device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            enumerator.AudioDeviceChanged +=
                new WeakEventHandler<AudioDeviceChangedEventArgs>(EnumeratorOnAudioDeviceChanged).Handler;
        }

        ~CoreAudioDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            GC.SuppressFinalize(this);
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
                if (_device == null)
                    return String.Empty;

                return _device.ID;
            }
        }

        public override string Description
        {
            get
            {
                if (_device == null)
                    return "Unknown";
                return _device.DeviceFriendlyName;
            }
        }

        public override string ShortName
        {
            get
            {
                if (_device == null)
                    return "Unknown";
                return _device.DeviceName;
            }
            set
            {
                _device.DeviceName = value;
            }
        }

        public override string SystemName
        {
            get
            {
                if (_device == null)
                    return "Unknown";
                return _device.SystemName;
            }
        }

        public override string FullName
        {
            get
            {
                if (_device != null)
                    return _device.DeviceFriendlyName;
                return "Unknown _device";
            }
        }

        public override string IconPath
        {
            get
            {
                if (_device != null)
                    return _device.IconPath;
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
                return _device.State.AsDeviceState();
            }
        }

        public override DeviceType DeviceType
        {
            get { return _device.EDataFlow.AsDeviceType(); }
        }

        public override bool IsMuted
        {
            get
            {
                if (_device.AudioEndpointVolume == null)
                    return false;

                return _device.AudioEndpointVolume.Mute;
            }
        }

        /// <summary>
        ///     The volume level on a scale between 0-100. Returns -1 if end point does not have volume
        /// </summary>
        public override int Volume
        {
            get
            {
                if (_device.AudioEndpointVolume == null)
                    return -1;

                return (int)Math.Round(_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0);
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;

                float val = (float)value / 100;

                if (_device.AudioEndpointVolume == null)
                    return;

                _device.AudioEndpointVolume.MasterVolumeLevelScalar = val;

                //Something is up with the floating point numbers in Windows, so make sure the volume is correct
                if (_device.AudioEndpointVolume.MasterVolumeLevelScalar < val)
                    _device.AudioEndpointVolume.MasterVolumeLevelScalar += 0.0001F;
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
            if (_device.AudioEndpointVolume == null)
                return false;

            _device.AudioEndpointVolume.Mute = true;
            return _device.AudioEndpointVolume.Mute;
        }

        /// <summary>
        ///     Unmute this device
        /// </summary>
        public override bool UnMute()
        {
            if (_device.AudioEndpointVolume == null)
                return false;

            _device.AudioEndpointVolume.Mute = false;
            return _device.AudioEndpointVolume.Mute;
        }

        public override event AudioDeviceChangedHandler VolumeChanged;

        /// <summary>
        ///     Extracts the unique GUID Identifier for a Windows System _device
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