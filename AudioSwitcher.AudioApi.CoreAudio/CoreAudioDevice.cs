using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice : Device
    {
        private static readonly Dictionary<PropertyKey, Expression<Func<IDevice, object>>> PropertykeyToLambdaMap = new Dictionary
            <PropertyKey, Expression<Func<IDevice, object>>>
        {
            {PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME, x => x.InterfaceName},
            {PropertyKeys.PKEY_DEVICE_DESCRIPTION, x => x.Name},
            {PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME, x => x.FullName},
            {PropertyKeys.PKEY_DEVICE_ICON, x => x.Icon}
        };

        private readonly ManualResetEvent _muteChangedResetEvent = new ManualResetEvent(false);
        private readonly AsyncManualResetEvent _volumeResetEvent = new AsyncManualResetEvent(false);
        private readonly AsyncManualResetEvent _defaultResetEvent = new AsyncManualResetEvent(false);
        private readonly IDisposable _peakValueTimerSubscription;
        private EDataFlow _dataFlow;
        private IMultimediaDevice _device;
        private Guid? _id;
        private bool _isDisposed;
        private bool _isMuted;
        private double _volume;

        private float _peakValue = -1;
        private CachedPropertyDictionary _properties;
        private string _realId;
        private EDeviceState _state;
        private bool _isUpdatingPeakValue;
        private bool _isDefaultDevice;
        private bool _isDefaultCommDevice;

        private IMultimediaDevice Device
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException("");

                return _device;
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
                return _isDefaultDevice;
            }
        }

        public override bool IsDefaultCommunicationsDevice
        {
            get
            {
                return _isDefaultCommDevice;
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
        public override double Volume
        {
            get
            {
                if (AudioEndpointVolume == null)
                    return -1;

                return _volume;
            }
            set
            {
                if (AudioEndpointVolume == null)
                    return;

                SetVolumeAsync(value).Wait();
            }
        }

        internal CoreAudioDevice(IMultimediaDevice device, CoreAudioController controller)
            : base(controller)
        {
            ComThread.Assert();

            _device = device;

            if (device == null)
                throw new ArgumentNullException(nameof(device));

            LoadProperties(device);

            ReloadAudioMeterInformation(device);
            ReloadAudioEndpointVolume(device);
            ReloadAudioSessionController(device);

            controller.SystemEvents.DeviceStateChanged
                                    .When(x => String.Equals(x.DeviceId, RealId, StringComparison.OrdinalIgnoreCase))
                                    .Subscribe(x => OnStateChanged(x.State));

            controller.SystemEvents.DefaultDeviceChanged
                                    .When(x =>
                                    {
                                        return x.DeviceRole != ERole.Multimedia && (String.Equals(x.DeviceId, RealId, StringComparison.OrdinalIgnoreCase) || _isDefaultCommDevice || _isDefaultDevice);
                                    })
                                    .Subscribe(x => OnDefaultChanged());

            controller.SystemEvents.PropertyChanged
                                    .When(x => String.Equals(x.DeviceId, RealId, StringComparison.OrdinalIgnoreCase))
                                    .Subscribe(x => OnPropertyChanged(x.PropertyKey));

            _peakValueTimerSubscription = PeakValueTimer.PeakValueTick.Subscribe(Timer_UpdatePeakValue);
        }

        ~CoreAudioDevice()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _peakValueTimerSubscription?.Dispose();

                ComThread.BeginInvoke(() =>
                {
                    ClearAudioEndpointVolume();
                    ClearAudioMeterInformation();
                    ClearAudioSession();

                    _device = null;
                });


                _isDisposed = true;
            }

            base.Dispose(disposing);
        }


        public override bool Mute(bool mute)
        {
            if (_isMuted == mute)
                return mute;

            if (AudioEndpointVolume == null)
                return true;

            AudioEndpointVolume.Mute = mute;

            //1s is a resonable response time for muting a device
            //any longer and we can assume that something went wrong
            _muteChangedResetEvent.WaitOne(1000);

            return _isMuted;
        }

        public override async Task<double> SetVolumeAsync(double volume)
        {
            if (_volume == volume)
                return _volume;

            if (AudioEndpointVolume == null)
                return -1;

            if (volume <= 0)
                volume = 0;
            else if (volume >= 100)
                volume = 100;
            else
                volume += 0.0001F;

            AudioEndpointVolume.MasterVolumeLevelScalar = (float)(volume / 100);

            await _volumeResetEvent.WaitAsync();

            return _volume;
        }

        public override bool SetAsDefault()
        {
            if (State != DeviceState.Active)
                return false;

            try
            {
                PolicyConfig.SetDefaultEndpoint(RealId, ERole.Console | ERole.Multimedia);

                _defaultResetEvent.Wait();

                return IsDefaultDevice;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> SetAsDefaultAsync()
        {
            if (State != DeviceState.Active)
                return false;

            try
            {
                PolicyConfig.SetDefaultEndpoint(RealId, ERole.Console | ERole.Multimedia);

                await _defaultResetEvent.WaitAsync();

                return IsDefaultDevice;
            }
            catch
            {
                return false;
            }
        }

        public override bool SetAsDefaultCommunications()
        {
            if (State != DeviceState.Active)
                return false;

            try
            {
                PolicyConfig.SetDefaultEndpoint(RealId, ERole.Console | ERole.Multimedia);

                _defaultResetEvent.Wait();

                return IsDefaultDevice;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> SetAsDefaultCommunicationsAsync()
        {
            if (State != DeviceState.Active)
                return false;

            try
            {
                PolicyConfig.SetDefaultEndpoint(RealId, ERole.Communications);

                await _defaultResetEvent.WaitAsync();
                
                return IsDefaultCommunicationsDevice;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnDefaultChanged()
        {
            _isDefaultCommDevice = Controller.GetDefaultDevice(DeviceType, Role.Communications)?.Id == Id;
            _isDefaultDevice = Controller.GetDefaultDevice(DeviceType, Role.Multimedia | Role.Console)?.Id == Id;

            _defaultResetEvent.Set();
            _defaultResetEvent.Reset();

            base.OnDefaultChanged();
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


        private void OnPropertyChanged(PropertyKey propertyKey)
        {
            ComThread.BeginInvoke(() =>
            {
                LoadProperties(Device);
            })
            .ContinueWith(x =>
            {
                //Ignore the properties we don't care about
                if (!PropertykeyToLambdaMap.ContainsKey(propertyKey))
                    return;

                var propertyName = DevicePropertyChangedArgs.FromExpression(this, PropertykeyToLambdaMap[propertyKey]).PropertyName;

                OnPropertyChanged(propertyName);
            });
        }

        private void OnStateChanged(EDeviceState deviceState)
        {
            _state = deviceState;

            ReloadAudioEndpointVolume(Device);
            ReloadAudioMeterInformation(Device);
            ReloadAudioSessionController(Device);

            OnStateChanged(deviceState.AsDeviceState());
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

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            _volume = data.MasterVolume * 100;
            _volumeResetEvent.Set();
            _volumeResetEvent.Reset();

            OnVolumeChanged(_volume);

            if (data.Muted != _isMuted)
            {
                _isMuted = data.Muted;
                _muteChangedResetEvent.Set();
                _muteChangedResetEvent.Reset();

                OnMuteChanged(_isMuted);
            }
        }

        private void Timer_UpdatePeakValue(long ticks)
        {
            if (_isUpdatingPeakValue)
                return;

            _isUpdatingPeakValue = true;

            var peakValue = _peakValue;

            if (_isDisposed)
                return;

            try
            {
                AudioMeterInformation.GetPeakValue(out peakValue);
            }
            catch
            {
                //ignored - usually means the com object has been released, but the timer is still ticking
            }

            if (Math.Abs(_peakValue - peakValue) > 0.001)
            {
                _peakValue = peakValue;
                OnPeakValueChanged(peakValue * 100);
            }

            _isUpdatingPeakValue = false;
        }

        /// <summary>
        ///     Extracts the unique GUID Identifier for a Windows System _device
        /// </summary>
        /// <param name="systemDeviceId"></param>
        /// <returns></returns>
        private static Guid SystemIdToGuid(string systemDeviceId)
        {
            return systemDeviceId.ExtractGuids().First();
        }
    }
}