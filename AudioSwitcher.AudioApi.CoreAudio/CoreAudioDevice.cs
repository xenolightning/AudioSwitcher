using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;
using Nito.AsyncEx;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice : Device
    {
        private const int DEFAULT_COM_TIMEOUT = 500;

        private static readonly Dictionary<PropertyKey, HashSet<string>> PropertykeyToPropertyMap = new Dictionary<PropertyKey, HashSet<string>>
        {
            {PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME, new HashSet<string>{ nameof(InterfaceName) }},
            {PropertyKeys.PKEY_DEVICE_DESCRIPTION, new HashSet<string>{ nameof(Name), nameof(FullName) }},
            {PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME, new HashSet<string>{ nameof(FullName) }},
            {PropertyKeys.PKEY_DEVICE_ICON, new HashSet<string>{ nameof(Icon), nameof(IconPath) }},
        };

        private readonly AsyncAutoResetEvent _muteChangedResetEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent _volumeResetEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent _defaultResetEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent _defaultCommResetEvent = new AsyncAutoResetEvent(false);
        private readonly IDisposable _peakValueTimerSubscription;
        private EDataFlow _dataFlow;
        private IMultimediaDevice _device;
        private readonly CoreAudioController _controller;
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

        public string RealId => _realId;

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

        public override DeviceIcon Icon => IconStringToDeviceIcon(IconPath);

        public override string IconPath
        {
            get
            {
                if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_ICON))
                    return Properties[PropertyKeys.PKEY_DEVICE_ICON] as string;

                return "Unknown";
            }
        }

        public override bool IsDefaultDevice => _isDefaultDevice;

        public override bool IsDefaultCommunicationsDevice => _isDefaultCommDevice;

        public override DeviceState State => _state.AsDeviceState();

        public override DeviceType DeviceType => _dataFlow.AsDeviceType();

        public override bool IsMuted => _isMuted;

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
            _controller = controller;

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
                                        //Ignore duplicate mm event
                                        if (x.DeviceRole == ERole.Multimedia)
                                            return false;

                                        if (String.Equals(x.DeviceId, RealId, StringComparison.OrdinalIgnoreCase))
                                            return true;

                                        //Ignore events for other device types
                                        if (!x.DataFlow.HasFlag(_dataFlow))
                                            return false;

                                        return _isDefaultCommDevice || _isDefaultDevice;
                                    })
                                    .Subscribe(x => OnDefaultChanged(x.DeviceRole));

            controller.SystemEvents.PropertyChanged
                                    .When(x => String.Equals(x.DeviceId, RealId, StringComparison.OrdinalIgnoreCase))
                                    .Subscribe(x => OnPropertyChanged(x.PropertyKey));

            _peakValueTimerSubscription = PeakValueTimer.PeakValueTick.Subscribe(Timer_UpdatePeakValue);
        }

        ~CoreAudioDevice()
        {
            Dispose(false);
        }

        internal void Dispose()
        {
            Dispose(true);
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
                return _isMuted;

            if (AudioEndpointVolume == null)
                return true;

            AudioEndpointVolume.Mute = mute;
            _muteChangedResetEvent.Wait(DEFAULT_COM_TIMEOUT);

            return _isMuted;
        }

        public override async Task<bool> MuteAsync(bool mute)
        {
            if (_isMuted == mute)
                return _isMuted;

            if (AudioEndpointVolume == null)
                return true;

            AudioEndpointVolume.Mute = mute;
            await _muteChangedResetEvent.WaitAsync(DEFAULT_COM_TIMEOUT);

            return _isMuted;
        }

        public override async Task<double> SetVolumeAsync(double volume)
        {
            if (Math.Abs(_volume - volume) < 0.1)
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
            await _volumeResetEvent.WaitAsync(DEFAULT_COM_TIMEOUT);

            return _volume;
        }

        public override bool SetAsDefault()
        {
            if (State != DeviceState.Active)
                return false;

            try
            {
                PolicyConfig.SetDefaultEndpoint(RealId, ERole.Console | ERole.Multimedia);
                _defaultResetEvent.Wait(DEFAULT_COM_TIMEOUT);

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
                await _defaultResetEvent.WaitAsync(DEFAULT_COM_TIMEOUT);

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
                PolicyConfig.SetDefaultEndpoint(RealId, ERole.Communications);
                _defaultCommResetEvent.Wait(DEFAULT_COM_TIMEOUT);

                return IsDefaultCommunicationsDevice;
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
                await _defaultCommResetEvent.WaitAsync(DEFAULT_COM_TIMEOUT);

                return IsDefaultCommunicationsDevice;
            }
            catch
            {
                return false;
            }
        }
        private void OnDefaultChanged(ERole deviceRole)
        {
            if (deviceRole == ERole.Communications)
            {
                _isDefaultCommDevice = Controller.GetDefaultDevice(DeviceType, Role.Communications)?.Id == Id;

                _defaultCommResetEvent.Set();

                if (_defaultCommResetEvent.IsSet)
                    _defaultCommResetEvent.Wait(50); //quick wait to consume the set above, and don't deadlock
            }
            else
            {
                _isDefaultDevice = Controller.GetDefaultDevice(DeviceType, Role.Multimedia | Role.Console)?.Id == Id;

                _defaultResetEvent.Set();
                if (_defaultResetEvent.IsSet)
                    _defaultResetEvent.Wait(50); //quick wait to consume the set above, and don't deadlock
            }

            OnDefaultChanged();
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

            //load the initial default state. Have to query using device id because this device is not cached until after creation
            _isDefaultCommDevice = _controller.GetDefaultDeviceId(DeviceType, Role.Communications) == RealId;
            _isDefaultDevice = _controller.GetDefaultDeviceId(DeviceType, Role.Multimedia | Role.Console) == RealId;
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
                if (!PropertykeyToPropertyMap.ContainsKey(propertyKey))
                    return;

                foreach (var propName in PropertykeyToPropertyMap[propertyKey])
                    OnPropertyChanged(propName);

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

            OnVolumeChanged(_volume);

            if (data.Muted != _isMuted)
            {
                _isMuted = data.Muted;
                _muteChangedResetEvent.Set();

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