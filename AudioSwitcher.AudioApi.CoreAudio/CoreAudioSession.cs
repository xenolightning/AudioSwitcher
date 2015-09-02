using System;
using System.Diagnostics;
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

        public uint ProcessId
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    uint processId;
                    _control.GetProcessId(out processId);
                    return processId;
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
                    return (AudioSessionState)state;
                });
            }
        }

        public CoreAudioSession(IAudioSessionControl control)
        {
            ComThread.Assert();

            _control = control as IAudioSessionControl2;
            _volume = control as ISimpleAudioVolume;

            _control.RegisterAudioSessionNotification(this);

            try
            {
                if (ProcessId > 0)
                {
                    var proc = Process.GetProcessById((int)ProcessId);
                    _fileDescription = proc.MainModule.FileVersionInfo.FileDescription;
                }
            }
            catch
            {
                _fileDescription = "";
            }
        }

        public int OnDisplayNameChanged(string displayName, ref Guid eventContext)
        {

            return 0;
        }

        public int OnIconPathChanged(string iconPath, ref Guid eventContext)
        {

            return 0;
        }

        public int OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
        {

            return 0;
        }

        public int OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex, ref Guid eventContext)
        {

            return 0;
        }

        public int OnGroupingParamChanged(ref Guid groupingId, ref Guid eventContext)
        {

            return 0;
        }

        public int OnStateChanged(EAudioSessionState state)
        {

            return 0;
        }

        public int OnSessionDisconnected(EAudioSessionDisconnectReason disconnectReason)
        {
            return 0;
        }
    }
}
