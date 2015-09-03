using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CoreAudioSession : IAudioSession, IAudioSessionEvents
    {
        private readonly IAudioSessionControl2 _control;
        private readonly ISimpleAudioVolume _volume;
        private readonly string _fileDescription;

        public string SessionId
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    string id;
                    _control.GetSessionIdentifier(out id);
                    return id;
                });
            }
        }

        public int ProcessId
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    uint processId;
                    _control.GetProcessId(out processId);
                    return (int)processId;
                });
            }
        }

        public string DisplayName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    string display;
                    _control.GetDisplayName(out display);
                    return String.IsNullOrEmpty(display) ? _fileDescription : display;
                });
            }
        }

        public bool IsSystemSession
        {
            get
            {
                return ComThread.Invoke(() => _control.IsSystemSoundsSession() == 0);
            }
        }

        public int Volume
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    float vol;
                    _volume.GetMasterVolume(out vol);
                    return (int)vol * 100;
                });
            }
            set
            {
                ComThread.Invoke(() =>
                {
                    _volume.SetMasterVolume(((float)value) / 100, Guid.Empty);
                });
            }
        }

        public AudioSessionState SessionState
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    EAudioSessionState state;
                    _control.GetState(out state);
                    return state.AsAudioSessionState();
                });
            }
        }

        public event SessionStateChangedEventHandler StateChanged;
        public event SessionDisconnectedEventHandler Disconnected;

        public CoreAudioSession(IAudioSessionControl control)
        {
            ComThread.Assert();

            // ReSharper disable once SuspiciousTypeConversion.Global
            _control = control as IAudioSessionControl2;

            // ReSharper disable once SuspiciousTypeConversion.Global
            _volume = control as ISimpleAudioVolume;

            if (_control == null || _volume == null)
                throw new InvalidComObjectException("control");

            _control.RegisterAudioSessionNotification(this);

            try
            {
                if (ProcessId > 0)
                {
                    var proc = Process.GetProcessById(ProcessId);
                    _fileDescription = proc.MainModule.FileVersionInfo.FileDescription;
                }
            }
            catch
            {
                _fileDescription = "";
            }
        }

        int IAudioSessionEvents.OnDisplayNameChanged(string displayName, ref Guid eventContext)
        {

            return 0;
        }

        int IAudioSessionEvents.OnIconPathChanged(string iconPath, ref Guid eventContext)
        {
            return 0;
        }

        int IAudioSessionEvents.OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
        {
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
            FireDisconnected();
            return 0;
        }

        int IAudioSessionEvents.OnStateChanged(EAudioSessionState state)
        {
            FireStateChanged(state);

            return 0;
        }

        private void FireStateChanged(EAudioSessionState state)
        {
            var handler = StateChanged;
            if (handler != null)
                handler(this, state.AsAudioSessionState());
        }

        private void FireDisconnected()
        {
            var handler = Disconnected;
            if (handler != null)
                handler(this);
        }

        public void Dispose()
        {
            Marshal.FinalReleaseComObject(_control);
        }
    }
}
