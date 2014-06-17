using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [ComVisible(false)]
    public sealed class CoreAudioDevice : Device, INotifyPropertyChanged
    {
        private Guid? _id;
        private MMDevice _device;

        internal CoreAudioDevice(MMDevice device, IDeviceEnumerator<CoreAudioDevice> enumerator)
            : base(enumerator)
        {
            if (device == null)
                throw new ArgumentNullException("device", "Device cannot be null. Something bad went wrong");

            ComThread.Invoke(() =>
            {
                Device = device;
                if (Device.AudioEndpointVolume != null)
                    Device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            });

            enumerator.AudioDeviceChanged += EnumeratorOnAudioDeviceChanged;
        }

        private void EnumeratorOnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs audioDeviceChangedEventArgs)
        {
            if (audioDeviceChangedEventArgs.Device.Id != this.Id)
                return;

            if (audioDeviceChangedEventArgs.EventType == AudioDeviceEventType.PropertyChanged)
            {
                OnPropertyChanged("DataFlow");
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
        /// <remarks>Has to remain private to correctly control access to the ComObject in non STA threads</remarks>
        private MMDevice Device
        {
            get
            {
                if (Thread.CurrentThread.ManagedThreadId != ComThread.TaskScheduler.ThreadId)
                    throw new ThreadStateException("Cross Thread access to COM Object MMDevice Detected");

                return _device;
            }
            set { _device = value; }
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

        public override string RealId
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device == null)
                        return String.Empty;

                    return Device.ID;
                });
            }
        }

        public override string Description
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device == null)
                        return "Unknown";
                    return Device.DeviceFriendlyName;
                });
            }
        }

        public override string ShortName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device == null)
                        return "Unknown";
                    return Device.DeviceName;
                });
            }
            set
            {
                ComThread.Invoke(() =>
                {
                    Device.DeviceName = value;
                });
            }
        }

        public override string SystemName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device == null)
                        return "Unknown";
                    return Device.SystemName;
                });
            }
        }

        public override string FullName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device != null)
                        return Device.DeviceFriendlyName;
                    return "Unknown Device";
                });
            }
        }

        public override string IconPath
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device != null)
                        return Device.IconPath;
                    return "Unknown";
                });
            }
        }

        public override bool IsDefaultDevice
        {
            get
            {
                return Enumerator.DefaultPlaybackDevice.Id == Id
                       || Enumerator.DefaultCaptureDevice.Id == Id;
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                return Enumerator.DefaultCommunicationsPlaybackDevice.Id == Id
                       || Enumerator.DefaultCommunicationsCaptureDevice.Id == Id;
            }
        }

        public override DeviceState State
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    return Device.State;
                });
            }
        }

        public override DataFlow DataFlow
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    return Device.DataFlow;
                });
            }
        }

        public override bool IsMuted
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Device.AudioEndpointVolume == null)
                        return false;

                    return Device.AudioEndpointVolume.Mute;
                });
            }
        }

        /// <summary>
        ///     The volume level on a scale between 0-100. Returns -1 if end point does not have volume
        /// </summary>
        public override int Volume
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    try
                    {
                        return (int)Math.Round(Device.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0);
                    }
                    catch
                    {
                        return -1;
                    }
                });
            }
            set
            {
                ComThread.Invoke(() =>
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
                });
            }
        }

        /// <summary>
        ///     Mute this device
        /// </summary>
        public override bool Mute()
        {
            return ComThread.Invoke(() =>
            {
                Device.AudioEndpointVolume.Mute = true;
                return Device.AudioEndpointVolume.Mute;
            });
        }

        /// <summary>
        ///     Unmute this device
        /// </summary>
        public override bool UnMute()
        {
            return ComThread.Invoke(() =>
            {
                Device.AudioEndpointVolume.Mute = false;
                return Device.AudioEndpointVolume.Mute;
            });
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}