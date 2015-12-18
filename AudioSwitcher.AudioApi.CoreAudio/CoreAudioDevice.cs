using System;
using System.Linq;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice : Device
    {
        private IMultimediaDevice _device;
        private Guid? _id;
        private CachedPropertyDictionary _properties;
        private EDeviceState _state;
        private string _realId;
        private bool _isMuted;
        private EDataFlow _dataFlow;
        private readonly IDisposable _changeSubscription;

        internal CoreAudioDevice(IMultimediaDevice device, IAudioController<CoreAudioDevice> controller)
            : base(controller)
        {
            ComThread.Assert();

            _device = device;

            if (device == null)
                throw new ArgumentNullException("device");

            LoadProperties(device);

            ReloadAudioMeterInformation(device);
            ReloadAudioEndpointVolume(device);
            ReloadAudioSessionController(device);

            _changeSubscription = controller.AudioDeviceChanged
                                                .When(x => x.Device != null && x.Device.Id == Id)
                                                .Subscribe(EnumeratorOnAudioDeviceChanged);
        }

        private void LoadProperties(IMultimediaDevice device)
        {
            ComThread.Assert();

            //Load values
            Marshal.ThrowExceptionForHR(device.GetId(out _realId));
            Marshal.ThrowExceptionForHR(device.GetState(out _state));

            // ReSharper disable once SuspiciousTypeConversion.Global
            var ep = device as IMultimediaEndpoint;
            if (ep != null)
                ep.GetDataFlow(out _dataFlow);

            GetPropertyInformation(device);
        }

        ~CoreAudioDevice()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            _changeSubscription.Dispose();

            ClearAudioEndpointVolume();
            ClearAudioMeterInformation();
            ClearAudioSession();

            _device = null;

            base.Dispose(disposing);

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
                return IconStringToDeviceIcon(IconPath);
            }
        }

        public override string IconPath
        {
            get
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_ICON))
                    return Properties[PropertyKeys.PKEY_DEVICE_ICON] as string;

                return "Unknown";
            }
        }

        public override bool IsDefaultDevice
        {
            get
            {
                IDevice defaultDevice = null;

                if (IsPlaybackDevice)
                    defaultDevice = Controller.DefaultPlaybackDevice;
                else if (IsCaptureDevice)
                    defaultDevice = Controller.DefaultCaptureDevice;

                return defaultDevice != null && defaultDevice.Id == Id;
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                IDevice defaultDevice = null;

                if (IsPlaybackDevice)
                    defaultDevice = Controller.DefaultPlaybackCommunicationsDevice;
                else if (IsCaptureDevice)
                    defaultDevice = Controller.DefaultCaptureCommunicationsDevice;

                return defaultDevice != null && defaultDevice.Id == Id;
            }
        }

        public override DeviceState State
        {
            get
            {
                return _state.AsDeviceState();
            }
        }

        public override DeviceType DeviceType
        {
            get
            {
                return _dataFlow.AsDeviceType();
            }
        }

        public override bool IsMuted
        {
            get
            {
                return _isMuted;
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

                var val = (float)value / 100;

                if (AudioEndpointVolume == null)
                    return;

                AudioEndpointVolume.MasterVolumeLevelScalar = val;

                //Something is up with the floating point numbers in Windows, so make sure the volume is correct
                if (AudioEndpointVolume.MasterVolumeLevelScalar < val)
                    AudioEndpointVolume.MasterVolumeLevelScalar += 0.0001F;
            }
        }

        private void EnumeratorOnAudioDeviceChanged(DeviceChangedArgs deviceChangedArgs)
        {
            var propertyChangedEvent = deviceChangedArgs as DevicePropertyChangedArgs;
            var stateChangedEvent = deviceChangedArgs as DeviceStateChangedArgs;
            var defaultChangedEvent = deviceChangedArgs as DefaultDeviceChangedArgs;

            if (propertyChangedEvent != null)
                HandlePropertyChanged(propertyChangedEvent);

            if (stateChangedEvent != null)
                HandleStateChanged(stateChangedEvent);

            if (defaultChangedEvent != null)
                HandleDefaultChanged();
        }

        private void HandlePropertyChanged(DevicePropertyChangedArgs propertyChanged)
        {
            ComThread.BeginInvoke(() =>
            {
                LoadProperties(_device);
            })
            .ContinueWith(x =>
            {
                OnPropertyChanged(propertyChanged.PropertyName);
            });
        }

        private void HandleStateChanged(DeviceStateChangedArgs stateChanged)
        {
            _state = stateChanged.State.AsEDeviceState();

            ReloadAudioEndpointVolume(_device);
            ReloadAudioMeterInformation(_device);
            ReloadAudioSessionController(_device);

            OnStateChanged(stateChanged.State);
        }

        private void ReloadAudioMeterInformation(IMultimediaDevice device)
        {
            ComThread.Invoke(() =>
            {
                LoadAudioMeterInformation(device);
            });
        }

        private void ReloadAudioSessionController(IMultimediaDevice device)
        {
            ComThread.Invoke(() =>
            {
                LoadAudioSessionController(device);
            });
        }

        private void ReloadAudioEndpointVolume(IMultimediaDevice device)
        {
            ComThread.Invoke(() =>
            {
                LoadAudioEndpointVolume(device);

                if (AudioEndpointVolume != null)
                    AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            });
        }

        private void HandleDefaultChanged()
        {
            OnDefaultChanged();
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            OnVolumeChanged(Volume);

            if (data.Muted != _isMuted)
            {
                _isMuted = data.Muted;
                OnMuteChanged(_isMuted);
            }

        }

        public override bool Mute(bool mute)
        {
            if (AudioEndpointVolume == null)
                return false;

            return AudioEndpointVolume.Mute = mute;
        }

        /// <summary>
        ///     Extracts the unique GUID Identifier for a Windows System _device
        /// </summary>
        /// <param name="systemDeviceId"></param>
        /// <returns></returns>
        public static Guid SystemIdToGuid(string systemDeviceId)
        {
            return systemDeviceId.ExtractGuids().First();
        }

    }
}