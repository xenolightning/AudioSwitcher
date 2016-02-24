using System;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary
    {
        public JavaScriptDeviceType DeviceType => _deviceType;

        public JavaScriptDeviceState DeviceState => _deviceState;

        public JavaScriptAudioDevice[] GetAudioDevices()
        {
            return GetAudioDevices(JavaScriptDeviceType.ALL);
        }

        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public JavaScriptAudioDevice[] GetAudioDevices(string type)
        {
            var devices = new List<IDevice>();

            switch (type.ToUpper())
            {
                case JavaScriptDeviceType.ALL:
                    devices.AddRange(AudioController.GetPlaybackDevices());
                    devices.AddRange(AudioController.GetCaptureDevices());
                    break;
                case JavaScriptDeviceType.PLAYBACK:
                    devices.AddRange(AudioController.GetPlaybackDevices());
                    break;
                case JavaScriptDeviceType.CAPTURE:
                    devices.AddRange(AudioController.GetCaptureDevices());
                    break;
            }

            //if empty then return empty array
            if (devices.Count == 0)
                return new JavaScriptAudioDevice[] {};

            return devices.Select(CreateJavaScriptAudioDevice).ToArray();
        }

        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <returns></returns>
        public JavaScriptAudioDevice[] GetPlaybackDevices()
        {
            var devices = new List<IDevice>();
            devices.AddRange(AudioController.GetPlaybackDevices());

            //if empty then return empty array
            if (devices.Count == 0)
                return new JavaScriptAudioDevice[] { };

            return devices.Select(CreateJavaScriptAudioDevice).ToArray();
        }

        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <returns></returns>
        public JavaScriptAudioDevice[] GetCaptureDevices()
        {
            var devices = new List<IDevice>();

            devices.AddRange(AudioController.GetCaptureDevices());

            //if empty then return empty array
            if (devices.Count == 0)
                return new JavaScriptAudioDevice[] { };

            return devices.Select(CreateJavaScriptAudioDevice).ToArray();
        }

        public JavaScriptAudioDevice GetAudioDevice(string name)
        {
            return GetAudioDevice(name, JavaScriptDeviceType.ALL);
        }

        /// <summary>
        ///     Get an audio device by name/id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JavaScriptAudioDevice GetAudioDevice(string name, string type)
        {
            IDevice device = null;

            Guid id;
            Guid.TryParse(name, out id);

            switch (type.ToUpper())
            {
                case JavaScriptDeviceType.ALL:
                    device = AudioController.GetDevices().FirstOrDefault(x => 
                        x.Id == id || 
                        String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case JavaScriptDeviceType.PLAYBACK:
                    device = AudioController.GetPlaybackDevices().FirstOrDefault(x => 
                        x.Id == id || 
                        String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case JavaScriptDeviceType.CAPTURE:
                    device = AudioController.GetCaptureDevices().FirstOrDefault(x => 
                        x.Id == id || 
                        String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    break;
            }

            return device != null ? CreateJavaScriptAudioDevice(device) : null;
        }

        /// <summary>
        ///     Returns the default device for the flag
        /// </summary>
        /// <param name="type">PLAYBACK, CAPTURE</param>
        /// <returns></returns>
        public JavaScriptAudioDevice GetDefaultDevice(string type)
        {
            switch (type)
            {
                case JavaScriptDeviceType.PLAYBACK:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultPlaybackDevice);
                case JavaScriptDeviceType.CAPTURE:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultCaptureDevice);
            }

            return null;
        }

        /// <summary>
        ///     Returns the default communication device for the flag
        /// </summary>
        /// <param name="type">PLAYBACK, CAPTURE</param>
        /// <returns></returns>
        public JavaScriptAudioDevice GetDefaultCommunicationDevice(string type)
        {
            switch (type.ToUpper())
            {
                case JavaScriptDeviceType.PLAYBACK:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultPlaybackCommunicationsDevice);
                case JavaScriptDeviceType.CAPTURE:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultCaptureCommunicationsDevice);
            }

            return null;
        }
    }
}