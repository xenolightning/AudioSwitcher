/*
  LICENSE
  -------
  Copyright (C) 2007 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
// modified for AudioSwitcher

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     MM Device
    /// </summary>
    internal class MMDevice : IDisposable
    {
        #region Variables

        private readonly IMMDevice _deviceInterface;
        private AudioEndpointVolume _audioEndpointVolume;
        private AudioMeterInformation _audioMeterInformation;
        private PropertyStore _propertyStore;

        #endregion

        #region Guids

        private static readonly Guid IID_I_AUDIO_METER_INFORMATION = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        private static readonly Guid IID_I_AUDIO_ENDPOINT_VOLUME = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
        private bool _audioMeterInformationUnavailable;
        private bool _audioEndpointVolumeUnavailable;

        #endregion

        #region Init

        private void GetPropertyInformation()
        {
            ComThread.Invoke(() =>
            {
                IPropertyStore propstore;
                try
                {
                    _deviceInterface.OpenPropertyStore(StorageAccessMode.ReadWrite, out propstore);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("Cannot open property store in write mode");
                    Marshal.ThrowExceptionForHR(_deviceInterface.OpenPropertyStore(StorageAccessMode.Read, out propstore));
                    _propertyStore = new PropertyStore(propstore, PropertyStore.Mode.Read);
                }
            });
        }

        private void GetAudioMeterInformation()
        {
            //Prevent further look ups
            if (_audioMeterInformationUnavailable)
                return;

            //This should be all on the COM thread to avoid any 
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Invoke(() =>
            {
                object result = null;
                Exception ex = null;

                //Need to catch here, as there is a chance that unauthorized is thrown.
                //It's not an HR exception, but bubbles up through the .net call stack
                try
                {
                    ex = Marshal.GetExceptionForHR(
                       _deviceInterface.Activate(IID_I_AUDIO_METER_INFORMATION, ClsCtx.ALL,
                           IntPtr.Zero, out result));
                }
                catch (UnauthorizedAccessException)
                {
                }

                _audioMeterInformationUnavailable = ex != null;

                if (_audioMeterInformationUnavailable)
                    return;

                _audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
            });
        }

        private void GetAudioEndpointVolume()
        {
            //Prevent further look ups
            if (_audioEndpointVolumeUnavailable)
                return;

            //Don't even bother looking up volume for disconnected devices
            if (State == EDeviceState.NotPresent || State == EDeviceState.Unplugged)
                return;

            //This should be all on the COM thread to avoid any 
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Invoke(() =>
            {
                object result = null;
                Exception ex = null;

                //Need to catch here, as there is a chance that unauthorized is thrown.
                //It's not an HR exception, but bubbles up through the .net call stack
                try
                {
                    ex = Marshal.GetExceptionForHR(
                       _deviceInterface.Activate(IID_I_AUDIO_ENDPOINT_VOLUME, ClsCtx.ALL,
                           IntPtr.Zero, out result));
                }
                catch (UnauthorizedAccessException)
                {
                }

                _audioEndpointVolumeUnavailable = ex != null;

                if (_audioEndpointVolumeUnavailable)
                    return;

                _audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Audio Meter Information
        /// </summary>
        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (_audioMeterInformation == null)
                        GetAudioMeterInformation();

                    return _audioMeterInformation;
                });
            }
        }

        /// <summary>
        ///     Audio Endpoint Volume
        /// </summary>
        public AudioEndpointVolume AudioEndpointVolume
        {
            get
            {
                if (_audioEndpointVolume == null)
                    GetAudioEndpointVolume();

                return _audioEndpointVolume;
            }
        }

        /// <summary>
        ///     Properties
        /// </summary>
        public PropertyStore Properties
        {
            get
            {
                if (_propertyStore == null)
                    GetPropertyInformation();

                return _propertyStore;
            }
        }

        /// <summary>
        ///     The name of the device. Eg. Speakers
        /// </summary>
        public string DeviceName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_DESCRIPTION))
                        return Properties[PropertyKeys.PKEY_DEVICE_DESCRIPTION].Value as string;

                    return DeviceInterfaceFriendlyName;
                });
            }
            set
            {
                ComThread.Invoke(() =>
                {
                    if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_DESCRIPTION))
                        Properties.SetValue(PropertyKeys.PKEY_DEVICE_DESCRIPTION, value);
                });
            }
        }

        /// <summary>
        ///     Friendly name of device. Eg. Realtek
        /// </summary>
        public string DeviceInterfaceFriendlyName
        {
            get
            {
                return ComThread.Invoke(() =>
                {

                    if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME))
                        return Properties[PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME].Value as string;

                    return "Unknown";
                });
            }
        }

        /// <summary>
        ///     Friendly name for the endpoint. Eg. Speakers (Realtek)
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME))
                        return Properties[PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME].Value as string;

                    return "Unknown";
                });
            }
        }

        /// <summary>
        ///     The Icon of the device
        /// </summary>
        public string IconPath
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    try
                    {
                        if (Properties != null && Properties.Contains(PropertyKeys.PKEY_DEVICE_ICON))
                        {
                            return (string)Properties[PropertyKeys.PKEY_DEVICE_ICON].Value;
                        }
                        return "Unknown";
                    }
                    catch
                    {
                        return "Unknown";
                    }
                });
            }
        }

        /// <summary>
        ///     Device ID
        /// </summary>
        public string Id
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    string result;
                    Marshal.ThrowExceptionForHR(_deviceInterface.GetId(out result));
                    return result;
                });
            }
        }

        /// <summary>
        ///     Data Flow
        /// </summary>
        public EDataFlow EDataFlow
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    EDataFlow result;
                    var ep = _deviceInterface as IMMEndpoint;
                    ep.GetDataFlow(out result);
                    return result;
                });
            }
        }

        /// <summary>
        ///     Device State
        /// </summary>
        public EDeviceState State
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    EDeviceState result;
                    Marshal.ThrowExceptionForHR(_deviceInterface.GetState(out result));
                    return result;
                });
            }
        }

        #endregion

        #region Constructor

        internal MMDevice(IMMDevice realDevice)
        {
            _deviceInterface = realDevice;
        }

        #endregion

        /// <summary>
        ///     To string
        /// </summary>
        public override string ToString()
        {
            return FriendlyName;
        }

        ~MMDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_audioEndpointVolume != null)
            {
                _audioEndpointVolume.Dispose();
                _audioEndpointVolume = null;
            }

            _audioMeterInformation = null;
            _propertyStore = null;

            GC.SuppressFinalize(this);
        }
    }
}