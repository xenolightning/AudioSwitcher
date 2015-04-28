using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice : Device, INotifyPropertyChanged, IDisposable
    {
        private IMMDevice _device;
        private Guid? _id;
        private CachedPropertyDictionary _properties;
        private EDeviceState _state;
        private string _realId;
        private EDataFlow _dataFlow;

        internal CoreAudioDevice(IMMDevice device, IAudioController<CoreAudioDevice> controller)
            : base(controller)
        {
            ComThread.Assert();

            _device = device;

            if (device == null)
                throw new ArgumentNullException("device");

            LoadProperties(device);

            GetAudioMeterInformation(device);
            GetAudioEndpointVolume(device);

            if (AudioEndpointVolume != null)
                AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            controller.AudioDeviceChanged +=
                new EventHandler<AudioDeviceChangedEventArgs>(EnumeratorOnAudioDeviceChanged)
                    .MakeWeak(x =>
                    {
                        controller.AudioDeviceChanged -= x;
                    });
        }

        private void LoadProperties(IMMDevice device)
        {
            ComThread.Assert();

            //Load values
            Marshal.ThrowExceptionForHR(device.GetId(out _realId));
            Marshal.ThrowExceptionForHR(device.GetState(out _state));

            // ReSharper disable once SuspiciousTypeConversion.Global
            var ep = device as IMMEndpoint;
            if (ep != null)
                ep.GetDataFlow(out _dataFlow);

            GetPropertyInformation(device);
        }

        ~CoreAudioDevice()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_audioEndpointVolume != null)
            {
                _audioEndpointVolume.Dispose();
                _audioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                _audioEndpointVolume = null;
            }

            if (_audioMeterInformation != null)
            {
                _audioMeterInformation.Dispose();
                _audioMeterInformation = null;
            }

            _device = null;
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
                return _realId;
            }
        }

        public override string InterfaceName
        {
            get
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME))
                    return Properties[PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME] as string;
                return "Unknown";
            }
        }

        /// <summary>
        /// The short name e.g. Speaker/Headphones etc..
        /// </summary>
        public override string Name
        {
            get
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_DESCRIPTION))
                    return Properties[PropertyKeys.PKEY_DEVICE_DESCRIPTION] as string;

                return InterfaceName;
            }
            set
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_DESCRIPTION))
                    Properties[PropertyKeys.PKEY_DEVICE_DESCRIPTION] = value;
            }
        }

        public override string FullName
        {
            get
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME))
                    return Properties[PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME] as string;
                return "Unknown";
            }
        }

        public override DeviceIcon Icon
        {
            get
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_ICON))
                    return IconStringToDeviceIcon(Properties[PropertyKeys.PKEY_DEVICE_ICON] as string);

                return DeviceIcon.Unknown;
            }
        }

        public override bool IsDefaultDevice
        {
            get
            {
                return (Controller.DefaultPlaybackDevice != null && Controller.DefaultPlaybackDevice.Id == Id)
                       || (Controller.DefaultCaptureDevice != null && Controller.DefaultCaptureDevice.Id == Id);
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                return (Controller.DefaultPlaybackCommunicationsDevice != null && Controller.DefaultPlaybackCommunicationsDevice.Id == Id)
                       || (Controller.DefaultCaptureCommunicationsDevice != null && Controller.DefaultCaptureCommunicationsDevice.Id == Id);
            }
        }

        public override DeviceState State
        {
            get { return _state.AsDeviceState(); }
        }

        public override DeviceType DeviceType
        {
            get { return _dataFlow.AsDeviceType(); }
        }

        public override bool IsMuted
        {
            get
            {
                if (AudioEndpointVolume == null)
                    return false;

                return AudioEndpointVolume.Mute;
            }
        }

        /// <summary>
        ///     The volume level on a scale between 0-100. Returns -1 if end point does not have volume
        /// </summary>
        public override int Volume
        {
            get
            {
                if (AudioEndpointVolume == null)
                    return -1;

                return (int)Math.Round(AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0);
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;

                float val = (float)value / 100;

                if (AudioEndpointVolume == null)
                    return;

                AudioEndpointVolume.MasterVolumeLevelScalar = val;

                //Something is up with the floating point numbers in Windows, so make sure the volume is correct
                if (AudioEndpointVolume.MasterVolumeLevelScalar < val)
                    AudioEndpointVolume.MasterVolumeLevelScalar += 0.0001F;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void EnumeratorOnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs audioDeviceChangedEventArgs)
        {
            if (audioDeviceChangedEventArgs.Device.Id != Id)
                return;

            ComThread.BeginInvoke(() =>
            {
                LoadProperties(_device);
            })
            .ContinueWith(x =>
            {
                switch (audioDeviceChangedEventArgs.EventType)
                {

                    case AudioDeviceEventType.Volume:
                    case AudioDeviceEventType.Level:
                    case AudioDeviceEventType.StateChanged:
                    case AudioDeviceEventType.PropertyChanged:
                        OnPropertyChanged("DeviceType");
                        OnPropertyChanged("InterfaceName");
                        OnPropertyChanged("FullName");
                        OnPropertyChanged("IconPath");
                        OnPropertyChanged("Id");
                        OnPropertyChanged("IsCaptureDevice");
                        OnPropertyChanged("IsMuted");
                        OnPropertyChanged("IsPlaybackDevice");
                        OnPropertyChanged("Name");
                        OnPropertyChanged("State");
                        OnPropertyChanged("FullName");
                        break;
                    case AudioDeviceEventType.DefaultDevice:
                        OnPropertyChanged("IsDefaultDevice");
                        break;
                    case AudioDeviceEventType.DefaultCommunicationsDevice:
                        OnPropertyChanged("IsDefaultCommunicationsDevice");
                        break;
                }
            });
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

        public override bool Mute(bool mute)
        {
            if (AudioEndpointVolume == null)
                return false;

            AudioEndpointVolume.Mute = mute;
            return AudioEndpointVolume.Mute;
        }

        public override event EventHandler<AudioDeviceChangedEventArgs> VolumeChanged;

        /// <summary>
        ///     Extracts the unique GUID Identifier for a Windows System _device
        /// </summary>
        /// <param name="systemDeviceId"></param>
        /// <returns></returns>
        public static Guid SystemIdToGuid(string systemDeviceId)
        {
            return systemDeviceId.ExtractGuids().First();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}