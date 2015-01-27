using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public sealed partial class CoreAudioDevice
    {
        private AudioMeterInformation _audioMeterInformation;
        private AudioEndpointVolume _audioEndpointVolume;
        private bool _audioMeterInformationUnavailable;
        private bool _audioEndpointVolumeUnavailable;

        private PropertyStore Properties
        {
            get
            {
                return _propertyStore;
            }
        }

        /// <summary>
        /// Audio Meter Information - Future support
        /// </summary>
        private AudioMeterInformation AudioMeterInformation
        {
            get
            {
                return _audioMeterInformation;
            }
        }
        /// <summary>
        /// Audio Endpoint Volume
        /// </summary>
        private AudioEndpointVolume AudioEndpointVolume
        {
            get
            {
                return _audioEndpointVolume;
            }
        }

        private void GetPropertyInformation(IMMDevice device)
        {
            ComThread.Assert();

            IPropertyStore propstore = null;
            //Opening in write mode, can cause exceptions to be thrown when not run as admin.
            //This tries to open in write mode if available
            try
            {
                device.OpenPropertyStore(StorageAccessMode.ReadWrite, out propstore);
            }
            catch
            {
                Debug.WriteLine("Cannot open property store in write mode");
            }

            if (propstore != null)
            {
                _propertyStore = new PropertyStore(propstore, PropertyStore.Mode.ReadWrite);
            }
            else
            {
                Marshal.ThrowExceptionForHR(device.OpenPropertyStore(StorageAccessMode.Read, out propstore));
                _propertyStore = new PropertyStore(propstore, PropertyStore.Mode.Read);
            }
        }

        private void GetAudioMeterInformation(IMMDevice device)
        {
            //Prevent further look ups
            if (_audioMeterInformationUnavailable)
                return;

            //This should be all on the COM thread to avoid any
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Assert();

            object result = null;
            Exception ex;
            //Need to catch here, as there is a chance that unauthorized is thrown.
            //It's not an HR exception, but bubbles up through the .net call stack
            try
            {
                var clsGuid = new Guid(ComIIds.AUDIO_METER_INFORMATION_IID);
                ex = Marshal.GetExceptionForHR(device.Activate(ref clsGuid, ClsCtx.Inproc, IntPtr.Zero, out result));
            }
            catch (Exception e)
            {
                ex = e;
            }
            _audioMeterInformationUnavailable = ex != null;
            if (_audioMeterInformationUnavailable)
                return;
            _audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }

        private void GetAudioEndpointVolume(IMMDevice device)
        {
            //Prevent further look ups
            if (_audioEndpointVolumeUnavailable)
                return;

            //Don't even bother looking up volume for disconnected devices
            if (State == DeviceState.NotPresent || State == DeviceState.Unplugged)
                return;

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
            _audioEndpointVolumeUnavailable = ex != null;
            if (_audioEndpointVolumeUnavailable)
                return;
            _audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
        }

        private static readonly Dictionary<string, DeviceIcon> ICON_MAP = new Dictionary<string, DeviceIcon>
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
            {"3051", DeviceIcon.Headphones},
        };

        private static DeviceIcon IconStringToDeviceIcon(string iconStr)
        {
            try
            {
                string imageKey = iconStr.Substring(iconStr.IndexOf(",") + 1).Replace("-", "");
                return ICON_MAP[imageKey];
            }
            catch
            {
                return DeviceIcon.Unknown;
            }
        }

    }
}
