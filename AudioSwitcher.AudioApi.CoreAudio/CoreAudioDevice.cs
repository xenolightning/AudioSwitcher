using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice : Device
    {
        private const int DefaultComTimeout = 500;

        private static readonly Dictionary<PropertyKey, HashSet<string>> PropertykeyToPropertyMap = new Dictionary<PropertyKey, HashSet<string>>
        {
            {PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME, new HashSet<string>{ nameof(InterfaceName) }},
            {PropertyKeys.PKEY_DEVICE_DESCRIPTION, new HashSet<string>{ nameof(Name), nameof(FullName) }},
            {PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME, new HashSet<string>{ nameof(FullName) }},
            {PropertyKeys.PKEY_DEVICE_ICON, new HashSet<string>{ nameof(Icon), nameof(IconPath) }},
        };

        private readonly SemaphoreSlim _setDefaultSemaphore = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _setDefaultCommSemaphore = new SemaphoreSlim(1);
        private readonly AutoResetEvent _muteChangedResetEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _volumeResetEvent = new AutoResetEvent(false);
        private readonly ManualResetEvent _defaultResetEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _defaultCommResetEvent = new ManualResetEvent(false);

        private IDisposable _peakValueTimerSubscription;
        private EDataFlow _dataFlow;
        private ThreadLocal<IMultimediaDevice> _device;
        private readonly CoreAudioController _controller;
        private Guid? _id;
        private double _volume;
        private float _peakValue = -1;
        private CachedPropertyDictionary _properties;
        private EDeviceState _state;
        private string _globalId;

        private volatile bool _isDisposed;
        private volatile bool _isMuted;
        private volatile bool _isUpdatingPeakValue;
        private volatile bool _isDefaultDevice;
        private volatile bool _isDefaultCommDevice;

        private IMultimediaDevice Device
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException("COM Device Disposed");

                return _device.Value;
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

        public string RealId => _globalId;

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

        public override IObservable<DevicePeakValueChangedArgs> PeakValueChanged
        {
            get
            {
                //Lazy initialization of the peak value timer subscription
                if (AudioMeterInformation != null && _peakValueTimerSubscription == null)
                    _peakValueTimerSubscription = PeakValueTimer.PeakValueTick.Subscribe(Timer_UpdatePeakValue);

                return base.PeakValueChanged;
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
        }

        internal CoreAudioDevice(IMultimediaDevice device, CoreAudioController controller)
            : base(controller)
        {
            ComThread.Assert();

            var devicePtr = Marshal.GetIUnknownForObject(device);
            _device = new ThreadLocal<IMultimediaDevice>(() => Marshal.GetUniqueObjectForIUnknown(devicePtr) as IMultimediaDevice);

            _controller = controller;

            if (device == null)
                throw new ArgumentNullException(nameof(device));

            LoadProperties();

            ReloadAudioMeterInformation();
            ReloadAudioEndpointVolume();
            ReloadAudioSessionController();

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
                                        if (x.DataFlow != _dataFlow)
                                            return false;

                                        return (x.DeviceRole == ERole.Communications && _isDefaultCommDevice) || (x.DeviceRole != ERole.Communications && _isDefaultDevice);
                                    })
                                    .Subscribe(x => OnDefaultChanged(x.DeviceId, x.DeviceRole));

            controller.SystemEvents.PropertyChanged
                                    .When(x => String.Equals(x.DeviceId, RealId, StringComparison.OrdinalIgnoreCase))
                                    .Subscribe(x => OnPropertyChanged(x.PropertyKey));
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
                _properties?.Dispose();
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
        public override bool HasCapability<TCapability>()
        {
            if (typeof(TCapability) == typeof(IAudioSessionController))
                return true;

            return false;
        }

        public override TCapability GetCapability<TCapability>()
        {
            if (_sessionController?.Value is TCapability)
                return (TCapability)(_sessionController?.Value as IDeviceCapability);

            return default(TCapability);
        }

        public override IEnumerable<IDeviceCapability> GetAllCapabilities()
        {
            yield return _sessionController?.Value;
        }

        public override async Task<bool> SetMuteAsync(bool mute, CancellationToken cancellationToken)
        {
            if (_isMuted == mute)
                return _isMuted;

            if (AudioEndpointVolume == null)
                return true;

            AudioEndpointVolume.Mute = mute;
            await _muteChangedResetEvent.WaitOneAsync(cancellationToken);

            return _isMuted;
        }

        public override Task<double> GetVolumeAsync(CancellationToken cancellationToken)
        {
            return TaskShim.FromResult(_volume);
        }

        public override async Task<double> SetVolumeAsync(double volume, CancellationToken cancellationToken)
        {
            if (AudioEndpointVolume == null)
                return -1;

            var normalizedVolume = volume.NormalizeVolume();

            if (Math.Abs(_volume - normalizedVolume) < 0.1)
                return _volume;

            AudioEndpointVolume.MasterVolumeLevelScalar = normalizedVolume;
            await _volumeResetEvent.WaitOneAsync(cancellationToken);

            return _volume;
        }

        public override bool SetAsDefault()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(DefaultComTimeout);

            return SetAsDefault(cts.Token);
        }

        public override bool SetAsDefault(CancellationToken cancellationToken)
        {
            if (State != DeviceState.Active)
                return _isDefaultDevice = false;

            var acquiredSemaphore = _setDefaultSemaphore.Wait(0, cancellationToken);

            try
            {
                if (acquiredSemaphore)
                {
                    _defaultResetEvent.Reset();
                    PolicyConfig.SetDefaultEndpoint(RealId, ERole.Console | ERole.Multimedia);
                }

                _defaultResetEvent.WaitOne(cancellationToken);
                return _isDefaultDevice;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TimeoutException)
            {
                throw new ComInteropTimeoutException(ex);
            }
            finally
            {
                if (acquiredSemaphore)
                    _setDefaultSemaphore.Release();
            }
        }

        public override Task<bool> SetAsDefaultAsync()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(DefaultComTimeout);

            return SetAsDefaultAsync(cts.Token);
        }

        public override async Task<bool> SetAsDefaultAsync(CancellationToken cancellationToken)
        {
            if (State != DeviceState.Active)
                return _isDefaultDevice = false;


            var acquiredSemaphore = await _setDefaultSemaphore.AvailableWaitHandle.WaitOneAsync(0, cancellationToken);

            try
            {
                if (acquiredSemaphore)
                {
                    _defaultResetEvent.Reset();
                    PolicyConfig.SetDefaultEndpoint(RealId, ERole.Console | ERole.Multimedia);
                }

                await _defaultResetEvent.WaitOneAsync(cancellationToken);
                return _isDefaultDevice;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TimeoutException)
            {
                throw new ComInteropTimeoutException(ex);
            }
            finally
            {
                if (acquiredSemaphore)
                    _setDefaultSemaphore.Release();
            }
        }

        public override bool SetAsDefaultCommunications()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(DefaultComTimeout);

            return SetAsDefaultCommunications(cts.Token);
        }

        public override bool SetAsDefaultCommunications(CancellationToken cancellationToken)
        {
            if (State != DeviceState.Active)
                return _isDefaultCommDevice = false;

            var acquiredSemaphore = _setDefaultCommSemaphore.Wait(0, cancellationToken);

            try
            {
                if (acquiredSemaphore)
                {
                    _defaultCommResetEvent.Reset();
                    PolicyConfig.SetDefaultEndpoint(RealId, ERole.Communications);
                }

                _defaultCommResetEvent.WaitOne(cancellationToken);
                return _isDefaultCommDevice;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TimeoutException)
            {
                throw new ComInteropTimeoutException(ex);
            }
            finally
            {
                if (acquiredSemaphore)
                    _setDefaultCommSemaphore.Release();
            }
        }

        public override Task<bool> SetAsDefaultCommunicationsAsync()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(DefaultComTimeout);

            return SetAsDefaultCommunicationsAsync(cts.Token);
        }

        public override async Task<bool> SetAsDefaultCommunicationsAsync(CancellationToken cancellationToken)
        {
            if (State != DeviceState.Active)
                return _isDefaultCommDevice = false;

            var acquiredSemaphore = await _setDefaultCommSemaphore.AvailableWaitHandle.WaitOneAsync(0, cancellationToken);

            try
            {
                if (acquiredSemaphore)
                {
                    _defaultCommResetEvent.Reset();
                    PolicyConfig.SetDefaultEndpoint(RealId, ERole.Communications);
                }

                await _defaultCommResetEvent.WaitOneAsync(cancellationToken);
                return _isDefaultCommDevice;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TimeoutException)
            {
                throw new ComInteropTimeoutException(ex);
            }
            finally
            {
                if (acquiredSemaphore)
                    _setDefaultCommSemaphore.Release();
            }
        }
        private void OnDefaultChanged(string deviceId, ERole deviceRole)
        {
            if (deviceRole == ERole.Communications)
            {
                _isDefaultCommDevice = String.Equals(deviceId, RealId, StringComparison.OrdinalIgnoreCase);
                _defaultCommResetEvent.Set();
            }
            else
            {
                _isDefaultDevice = String.Equals(deviceId, RealId, StringComparison.OrdinalIgnoreCase);
                _defaultResetEvent.Set();
            }

            OnDefaultChanged();
        }

        private void LoadProperties()
        {
            //If either of these error, assume that the device is in an unusable state
            Marshal.ThrowExceptionForHR(Device.GetState(out _state));
            Marshal.ThrowExceptionForHR(Device.GetId(out _globalId));

            // ReSharper disable once SuspiciousTypeConversion.Global
            var ep = Device as IMultimediaEndpoint;
            ep?.GetDataFlow(out _dataFlow);

            //load the initial default state. Have to query using device id because this device is not cached until after creation
            _isDefaultCommDevice = _controller.GetDefaultDeviceId(DeviceType, Role.Communications) == RealId;
            _isDefaultDevice = _controller.GetDefaultDeviceId(DeviceType, Role.Multimedia | Role.Console) == RealId;

            GetPropertyInformation(Device);
        }


        private void OnPropertyChanged(PropertyKey propertyKey)
        {
            ComThread.BeginInvoke(LoadProperties).ContinueWith(x =>
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

            ReloadAudioEndpointVolume();
            ReloadAudioMeterInformation();
            ReloadAudioSessionController();

            OnStateChanged(deviceState.AsDeviceState());

            //only load properties if it's active
            if ((deviceState & EDeviceState.Active) != 0)
            {
                //Attempt to reload properties if the device has become active.
                //A fail-safe incase the device doesn't fire property changed events
                //when it becomes "active"
                ComThread.BeginInvoke(LoadProperties);
            }
        }

        private void ReloadAudioMeterInformation()
        {
            ComThread.Invoke(LoadAudioMeterInformation);
        }

        private void ReloadAudioSessionController()
        {
            ComThread.Invoke(LoadAudioSessionController);
        }

        private void ReloadAudioEndpointVolume()
        {
            ComThread.Invoke(() =>
            {
                LoadAudioEndpointVolume();

                if (AudioEndpointVolume != null)
                    AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            });
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            _volume = data.MasterVolume.DeNormalizeVolume();
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
            if (_isUpdatingPeakValue || AudioMeterInformation == null)
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

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("CoreAudioDevice");
        }
    }
}