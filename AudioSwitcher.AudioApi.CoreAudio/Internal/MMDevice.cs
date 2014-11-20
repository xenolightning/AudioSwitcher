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

        private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
        private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");
        private bool _audioMeterInformationUnavailable = false;
        private bool _audioEndpointVolumeUnavailable = false;

        #endregion

        #region Init

        private void GetPropertyInformation()
        {
            ComThread.Invoke(() =>
            {
                IPropertyStore propstore;
                try
                {
                    Marshal.ThrowExceptionForHR(_deviceInterface.OpenPropertyStore(StorageAccessMode.ReadWrite,
                        out propstore));
                    _propertyStore = new PropertyStore(propstore, PropertyStore.Mode.ReadWrite);
                }
                catch
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

            object result = null;
            var ex =
                ComThread.Invoke(() =>
                {
                    return Marshal.GetExceptionForHR(_deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL,
                        IntPtr.Zero, out result));
                });

            _audioMeterInformationUnavailable = ex != null;

            if (_audioMeterInformationUnavailable)
                return;

            _audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }

        private void GetAudioEndpointVolume()
        {
            //Prevent further look ups
            if (_audioEndpointVolumeUnavailable)
                return;

            object result = null;
            if (State == EDeviceState.NotPresent || State == EDeviceState.Unplugged)
                return;

            var ex =
                ComThread.Invoke(() =>
                {
                    return Marshal.GetExceptionForHR(_deviceInterface.Activate(ref IID_IAudioEndpointVolume, ClsCtx.ALL,
                        IntPtr.Zero, out result));
                });

            _audioEndpointVolumeUnavailable = ex != null;

            if (_audioEndpointVolumeUnavailable)
                return;

            _audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
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
                return ComThread.Invoke(() =>
                {
                    if (_propertyStore == null)
                        GetPropertyInformation();
                    return _propertyStore;
                });
            }
        }

        /// <summary>
        ///     Friendly name for the endpoint
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (_propertyStore == null)
                        GetPropertyInformation();

                    if (_propertyStore != null && _propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName))
                    {
                        return (string)_propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
                    }
                    return "Unknown";
                });
            }
        }

        /// <summary>
        ///     Friendly name of device
        /// </summary>
        public string DeviceFriendlyName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (_propertyStore == null)
                        GetPropertyInformation();

                    if (_propertyStore != null &&
                        _propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
                    {
                        return (string)_propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
                    }
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
                        if (_propertyStore == null)
                            GetPropertyInformation();

                        if (_propertyStore != null && _propertyStore.Contains(PropertyKeys.PKEY_Device_Icon))
                        {
                            return (string)_propertyStore[PropertyKeys.PKEY_Device_Icon].Value;
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
        ///     The name fo the device
        /// </summary>
        public string DeviceName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (_propertyStore == null)
                        GetPropertyInformation();

                    if (_propertyStore != null && _propertyStore.Contains(PropertyKeys.PKEY_Device_Description))
                    {
                        return (string)_propertyStore[PropertyKeys.PKEY_Device_Description].Value;
                    }
                    return DeviceFriendlyName;
                });
            }
            set
            {
                ComThread.Invoke(() =>
                {
                    if (_propertyStore == null)
                        GetPropertyInformation();

                    if (_propertyStore != null && _propertyStore.Contains(PropertyKeys.PKEY_Device_Description))
                    {
                        _propertyStore.SetValue(PropertyKeys.PKEY_Device_Description, value);
                        //var pi = propertyStore[PropertyKeys.PKEY_Device_Description];
                        //propertyStore[PropertyKeys.PKEY_Device_Description] = pi;
                    }
                });
            }
        }

        /// <summary>
        ///     Friendly name of device
        /// </summary>
        public string SystemName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    if (_propertyStore == null)
                        GetPropertyInformation();

                    if (_propertyStore != null && _propertyStore.Contains(PropertyKeys.PKEY_System_Name))
                    {
                        return (string)_propertyStore[PropertyKeys.PKEY_System_Name].Value;
                    }
                    return "Unknown";
                });
            }
        }

        /// <summary>
        ///     Fully qualified name of the device
        /// </summary>
        public string FullName
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    try
                    {
                        if (_propertyStore == null)
                            GetPropertyInformation();

                        if (_propertyStore != null && (_propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName) &&
                                                       _propertyStore.Contains(
                                                           PropertyKeys.PKEY_DeviceInterface_FriendlyName)))
                        {
                            return _propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value + " (" +
                                   _propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value + ")";
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
        public string ID
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
            //_deviceInterface = null;

            GC.SuppressFinalize(this);
        }
    }
}