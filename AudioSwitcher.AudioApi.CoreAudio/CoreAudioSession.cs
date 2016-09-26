using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using Nito.AsyncEx;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class CoreAudioSession : IAudioSession, IAudioSessionEvents
    {
        private readonly IDisposable _deviceMutedSubscription;
        private readonly ThreadLocal<IAudioMeterInformation> _meterInformation;
        private readonly ThreadLocal<ISimpleAudioVolume> _simpleAudioVolume;
        private readonly ThreadLocal<IAudioSessionControl2> _audioSessionControl;
        private readonly IDisposable _timerSubscription;
        private readonly Broadcaster<SessionDisconnectedArgs> _disconnected;
        private readonly Broadcaster<SessionMuteChangedArgs> _muteChanged;
        private readonly Broadcaster<SessionPeakValueChangedArgs> _peakValueChanged;
        private readonly Broadcaster<SessionStateChangedArgs> _stateChanged;
        private readonly Broadcaster<SessionVolumeChangedArgs> _volumeChanged;
        private readonly AsyncAutoResetEvent _volumeResetEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent _muteResetEvent = new AsyncAutoResetEvent(false);
        private string _displayName;
        private string _executablePath;
        private string _fileDescription;
        private string _iconPath;
        private string _id;
        private bool _isDisposed;
        private bool _isMuted;
        private bool _isSystemSession;
        private volatile IntPtr _controlPtr;

        private float _peakValue = -1;
        private int _processId;
        private AudioSessionState _state;
        private double _volume;
        private bool _isUpdatingPeakValue;

        public CoreAudioSession(CoreAudioDevice device, IAudioSessionControl control)
        {
            ComThread.Assert();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var audioSessionControl = control as IAudioSessionControl2;

            // ReSharper disable once SuspiciousTypeConversion.Global
            var simpleAudioVolume = control as ISimpleAudioVolume;

            if (audioSessionControl == null || simpleAudioVolume == null)
                throw new InvalidComObjectException("control");

            _controlPtr = Marshal.GetIUnknownForObject(control);
            _audioSessionControl = new ThreadLocal<IAudioSessionControl2>(() => Marshal.GetUniqueObjectForIUnknown(_controlPtr) as IAudioSessionControl2, true);
            _meterInformation = new ThreadLocal<IAudioMeterInformation>(() => Marshal.GetUniqueObjectForIUnknown(_controlPtr) as IAudioMeterInformation, true);
            _simpleAudioVolume = new ThreadLocal<ISimpleAudioVolume>(() => Marshal.GetUniqueObjectForIUnknown(_controlPtr) as ISimpleAudioVolume, true);


            Device = device;

            _deviceMutedSubscription = Device.MuteChanged.Subscribe(x =>
            {
                OnMuteChanged(_isMuted);
            });


            if (_meterInformation != null)
            {
                //start a timer to poll for peak value changes
                _timerSubscription = PeakValueTimer.PeakValueTick.Subscribe(Timer_UpdatePeakValue);
            }

            _stateChanged = new Broadcaster<SessionStateChangedArgs>();
            _disconnected = new Broadcaster<SessionDisconnectedArgs>();
            _volumeChanged = new Broadcaster<SessionVolumeChangedArgs>();
            _muteChanged = new Broadcaster<SessionMuteChangedArgs>();
            _peakValueChanged = new Broadcaster<SessionPeakValueChangedArgs>();

            AudioSessionControl.RegisterAudioSessionNotification(this);

            RefreshProperties();
            RefreshVolume();
        }

        ~CoreAudioSession()
        {
            Dispose(false);
        }

        internal void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _timerSubscription?.Dispose();
            _audioSessionControl.Dispose();
            _meterInformation.Dispose();
            _simpleAudioVolume.Dispose();
            _deviceMutedSubscription.Dispose();
            _muteChanged.Dispose();
            _stateChanged.Dispose();
            _disconnected.Dispose();
            _volumeChanged.Dispose();
            _peakValueChanged.Dispose();

            //Run this on the com thread to ensure it's disposed correctly
            ComThread.BeginInvoke(() =>
            {
                AudioSessionControl.UnregisterAudioSessionNotification(this);
            });

            GC.SuppressFinalize(this);

            _isDisposed = true;
        }

        public IDevice Device { get; }

        public IObservable<SessionVolumeChangedArgs> VolumeChanged => _volumeChanged.AsObservable();

        public IObservable<SessionPeakValueChangedArgs> PeakValueChanged => _peakValueChanged.AsObservable();

        public IObservable<SessionMuteChangedArgs> MuteChanged => _muteChanged.AsObservable();

        public IObservable<SessionStateChangedArgs> StateChanged => _stateChanged.AsObservable();

        public IObservable<SessionDisconnectedArgs> Disconnected => _disconnected.AsObservable();
        public Task<double> GetVolumeAsync()
        {
            return GetVolumeAsync(CancellationToken.None);
        }

        public Task<double> GetVolumeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_volume);
        }

        public async Task<double> SetVolumeAsync(double volume)
        {
            return await SetVolumeAsync(volume, CancellationToken.None);
        }

        public async Task<double> SetVolumeAsync(double volume, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var normalizedVolume = volume.NormalizeVolume();

            ComThread.Invoke(() => SimpleAudioVolume.SetMasterVolume(normalizedVolume, Guid.Empty));

            await _volumeResetEvent.WaitAsync(cancellationToken);

            return _volume;
        }

        public Task<bool> GetMuteAsync()
        {
            return GetMuteAsync(CancellationToken.None);
        }

        public Task<bool> GetMuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_isMuted);
        }

        public async Task<bool> SetMuteAsync(bool muted)
        {
            return await SetMuteAsync(muted, CancellationToken.None);
        }

        public async Task<bool> SetMuteAsync(bool muted, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_isMuted == muted)
                return _isMuted;

            ComThread.Invoke(() => SimpleAudioVolume.SetMute(muted, Guid.Empty));

            await _muteResetEvent.WaitAsync(cancellationToken);

            return _isMuted;
        }

        public string Id => _id;

        public int ProcessId => _processId;

        public string DisplayName => String.IsNullOrWhiteSpace(_displayName) ? _fileDescription : _displayName;

        public string IconPath => _iconPath;

        public string ExecutablePath => _executablePath;

        public bool IsSystemSession => _isSystemSession;

        public double Volume => _volume;

        public bool IsMuted => _isMuted || Device.IsMuted;

        public AudioSessionState SessionState => _state;

        private IAudioMeterInformation MeterInformation => _meterInformation.Value;

        private ISimpleAudioVolume SimpleAudioVolume => _simpleAudioVolume.Value;

        private IAudioSessionControl2 AudioSessionControl => _audioSessionControl.Value;

        int IAudioSessionEvents.OnDisplayNameChanged(string displayName, ref Guid eventContext)
        {
            _displayName = displayName;
            return 0;
        }

        int IAudioSessionEvents.OnIconPathChanged(string iconPath, ref Guid eventContext)
        {
            _iconPath = iconPath;
            return 0;
        }

        int IAudioSessionEvents.OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
        {
            var adjustedVolume = volume*100;
            if (Math.Abs(_volume - adjustedVolume) > 0)
            {
                _volume = adjustedVolume;
                OnVolumeChanged(_volume);
            }

            if (isMuted != _isMuted)
            {
                _isMuted = isMuted;
                OnMuteChanged(_isMuted);
            }

            _volumeResetEvent.Set();
            _muteResetEvent.Set();

            return 0;
        }

        int IAudioSessionEvents.OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex, ref Guid eventContext)
        {
            return 0;
        }

        int IAudioSessionEvents.OnGroupingParamChanged(ref Guid groupingId, ref Guid eventContext)
        {
            return 0;
        }

        int IAudioSessionEvents.OnSessionDisconnected(EAudioSessionDisconnectReason disconnectReason)
        {
            OnDisconnected();
            return 0;
        }

        int IAudioSessionEvents.OnStateChanged(EAudioSessionState state)
        {
            _state = state.AsAudioSessionState();
            OnStateChanged(state);
            return 0;
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
                if (_meterInformation == null)
                    return;

                MeterInformation.GetPeakValue(out peakValue);
            }
            catch (InvalidComObjectException)
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

        private void RefreshVolume()
        {
            if (_isDisposed)
                return;

            ComThread.Invoke(() =>
            {
                float vol;
                SimpleAudioVolume.GetMasterVolume(out vol);
                _volume = vol * 100;

                bool isMuted;
                SimpleAudioVolume.GetMute(out isMuted);

                _isMuted = isMuted;
            });
        }

        private void RefreshProperties()
        {
            if (_isDisposed)
                return;

            ComThread.Invoke(() =>
            {
                _isSystemSession = AudioSessionControl.IsSystemSoundsSession() == 0;
                AudioSessionControl.GetDisplayName(out _displayName);

                AudioSessionControl.GetIconPath(out _iconPath);

                EAudioSessionState state;
                AudioSessionControl.GetState(out state);
                _state = state.AsAudioSessionState();

                uint processId;
                AudioSessionControl.GetProcessId(out processId);
                _processId = (int)processId;

                AudioSessionControl.GetSessionIdentifier(out _id);

                try
                {
                    if (ProcessId > 0)
                    {
                        var proc = Process.GetProcessById(ProcessId);
                        _executablePath = proc.MainModule.FileName;
                        _fileDescription = proc.MainModule.FileVersionInfo.FileDescription;
                    }
                }
                catch
                {
                    _fileDescription = "";
                }
            });
        }

        private void OnVolumeChanged(double volume)
        {
            _volumeChanged.OnNext(new SessionVolumeChangedArgs(this, volume));
        }

        private void OnStateChanged(EAudioSessionState state)
        {
            _stateChanged.OnNext(new SessionStateChangedArgs(this, state.AsAudioSessionState()));
        }

        private void OnDisconnected()
        {
            _disconnected.OnNext(new SessionDisconnectedArgs(this));
        }

        private void OnMuteChanged(bool isMuted)
        {
            _muteChanged.OnNext(new SessionMuteChangedArgs(this, isMuted));
        }

        private void OnPeakValueChanged(double peakValue)
        {
            _peakValueChanged.OnNext(new SessionPeakValueChangedArgs(this, peakValue));
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Session is disposed");
        }
    }
}
