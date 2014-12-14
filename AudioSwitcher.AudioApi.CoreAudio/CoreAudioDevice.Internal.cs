using System;
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
            ComThread.Invoke(() =>
            {
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
            });
        }

        private void GetAudioMeterInformation(IMMDevice device)
        {
            //Prevent further look ups
            if (_audioMeterInformationUnavailable)
                return;

            //This should be all on the COM thread to avoid any
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Invoke(() =>
            {
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
            });
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
            ComThread.Invoke(() =>
            {
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
            });
        }


    }
}
