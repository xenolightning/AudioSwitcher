using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice
    {
        private static readonly Dictionary<string, DeviceIcon> IconMap = new Dictionary<string, DeviceIcon>
        {
            {"0", DeviceIcon.Speakers},
            {"1", DeviceIcon.Speakers},
            {"2", DeviceIcon.Headphones},
            {"3", DeviceIcon.LineIn},
            {"4", DeviceIcon.Digital},
            {"5", DeviceIcon.DesktopMicrophone},
            {"6", DeviceIcon.Headset},
            {"7", DeviceIcon.Phone},
            {"8", DeviceIcon.Monitor},
            {"9", DeviceIcon.StereoMix},
            {"10", DeviceIcon.Speakers},
            {"11", DeviceIcon.Kinect},
            {"12", DeviceIcon.DesktopMicrophone},
            {"13", DeviceIcon.Speakers},
            {"14", DeviceIcon.Headphones},
            {"15", DeviceIcon.Speakers},
            {"16", DeviceIcon.Headphones},
            {"3004", DeviceIcon.Speakers},
            {"3010", DeviceIcon.Speakers},
            {"3011", DeviceIcon.Headphones},
            {"3012", DeviceIcon.LineIn},
            {"3013", DeviceIcon.Digital},
            {"3014", DeviceIcon.DesktopMicrophone},
            {"3015", DeviceIcon.Headset},
            {"3016", DeviceIcon.Phone},
            {"3017", DeviceIcon.Monitor},
            {"3018", DeviceIcon.StereoMix},
            {"3019", DeviceIcon.Speakers},
            {"3020", DeviceIcon.Kinect},
            {"3021", DeviceIcon.DesktopMicrophone},
            {"3030", DeviceIcon.Speakers},
            {"3031", DeviceIcon.Headphones},
            {"3050", DeviceIcon.Speakers},
            {"3051", DeviceIcon.Headphones}
        };

        private AudioEndpointVolume _audioEndpointVolume;
        private IAudioMeterInformation _audioMeterInformation;

        private IPropertyDictionary Properties
        {
            get
            {
                if(_isDisposed)
                    throw new ObjectDisposedException("");

                return _properties;
            }
        }

        /// <summary>
        /// Audio Meter Information - Future support
        /// </summary>
        private IAudioMeterInformation AudioMeterInformation
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException("");

                return _audioMeterInformation;
            }
        }

        /// <summary>
        /// Audio Endpoint VolumeChanged
        /// </summary>
        private AudioEndpointVolume AudioEndpointVolume => _audioEndpointVolume;

        private void GetPropertyInformation(IMultimediaDevice device)
        {
            ComThread.Assert();

            if (_properties == null)
                _properties = new CachedPropertyDictionary();

            //Don't try to load properties for a device that doesn't exist
            if (State == DeviceState.NotPresent)
                return;

            _properties.TryLoadFrom(device);
        }

        private void LoadAudioMeterInformation(IMultimediaDevice device)
        {
            //This should be all on the COM thread to avoid any
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Assert();

            Exception ex;
            //Need to catch here, as there is a chance that unauthorized is thrown.
            //It's not an HR exception, but bubbles up through the .net call stack
            try
            {
                var clsGuid = new Guid(ComIIds.AUDIO_METER_INFORMATION_IID);
                object result;
                ex = Marshal.GetExceptionForHR(device.Activate(ref clsGuid, ClsCtx.Inproc, IntPtr.Zero, out result));
                _audioMeterInformation = result as IAudioMeterInformation;
            }
            catch (Exception e)
            {
                ex = e;
            }

            if (ex != null)
                ClearAudioMeterInformation();
        }

        private void LoadAudioEndpointVolume(IMultimediaDevice device)
        {
            //Don't even bother looking up volume for disconnected devices
            if (!State.HasFlag(DeviceState.Active))
            {
                ClearAudioEndpointVolume();
                return;
            }

            //This should be all on the COM thread to avoid any
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Assert();

            object result = null;
            Exception ex;
            //Need to catch here, as there is a chance that unauthorized is thrown.
            //It's not an HR exception, but bubbles up through the .net call stack
            try
            {
                var clsGuid = new Guid(ComIIds.AUDIO_ENDPOINT_VOLUME_IID);
                ex = Marshal.GetExceptionForHR(device.Activate(ref clsGuid, ClsCtx.Inproc, IntPtr.Zero, out result));
            }
            catch (Exception e)
            {
                ex = e;
            }

            if (ex != null)
            {
                ClearAudioEndpointVolume();
                return;
            }

            _audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
            _isMuted = _audioEndpointVolume.Mute;
            _volume = _audioEndpointVolume.MasterVolumeLevelScalar*100;
        }

        private void ClearAudioEndpointVolume()
        {
            if (_audioEndpointVolume != null)
            {
                _audioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                _audioEndpointVolume.Dispose();
                _audioEndpointVolume = null;
                _volume = -1;
            }
        }

        private void ClearAudioMeterInformation()
        {
            if (_audioMeterInformation != null)
            {
                Marshal.FinalReleaseComObject(_audioMeterInformation);
                _audioMeterInformation = null;
            }
        }

        private static DeviceIcon IconStringToDeviceIcon(string iconStr)
        {
            try
            {
                var imageKey = iconStr.Substring(iconStr.IndexOf(",", StringComparison.InvariantCultureIgnoreCase) + 1).Replace("-", "");
                return IconMap[imageKey];
            }
            catch
            {
                return DeviceIcon.Unknown;
            }
        }
    }
}