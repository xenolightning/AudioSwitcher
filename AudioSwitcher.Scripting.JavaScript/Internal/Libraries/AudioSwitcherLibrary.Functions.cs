using System;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary
    {
        [JSProperty(Name = "DeviceType")]
        public JavaScriptDeviceType DeviceType
        {
            get { return _deviceType; }
        }

        [JSProperty(Name = "DeviceState")]
        public JavaScriptDeviceState DeviceState
        {
            get { return _deviceState; }
        }

        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [JSFunction(Name = "getAudioDevices")]
        public ArrayInstance GetAudioDevices([DefaultParameterValue(JavaScriptDeviceType.ALL)] string type = JavaScriptDeviceType.ALL)
        {
            var devices = new List<IDevice>();

            switch (type)
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
                return Engine.Array.New();

            return Engine.EnumerableToArray(devices.Select(CreateJavaScriptAudioDevice));
        }

        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <returns></returns>
        [JSFunction(Name = "getPlaybackDevices")]
        public ArrayInstance GetPlaybackDevices()
        {
            var devices = new List<IDevice>();
            devices.AddRange(AudioController.GetCaptureDevices());

            //if empty then return empty array
            if (devices.Count == 0)
                return Engine.Array.New();

            return Engine.EnumerableToArray(devices.Select(CreateJavaScriptAudioDevice));
        }

        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <returns></returns>
        [JSFunction(Name = "getCaptureDevices")]
        public ArrayInstance GetCaptureDevices()
        {
            var devices = new List<IDevice>();

            devices.AddRange(AudioController.GetCaptureDevices());

            //if empty then return empty array
            if (devices.Count == 0)
                return Engine.Array.New();

            return Engine.EnumerableToArray(devices.Select(CreateJavaScriptAudioDevice));
        }

        /// <summary>
        ///     Get an audio device by name/id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [JSFunction(Name = "getAudioDevice")]
        public JavaScriptAudioDevice GetAudioDevice(string name, [DefaultParameterValue(JavaScriptDeviceType.ALL)] string type = JavaScriptDeviceType.ALL)
        {
            IDevice device = null;

            Guid id;
            Guid.TryParse(name, out id);

            switch (type)
            {
                case JavaScriptDeviceType.ALL:
                    device = AudioController.GetDevices().FirstOrDefault(x => x.Id == id || String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case JavaScriptDeviceType.PLAYBACK:
                    device = AudioController.GetPlaybackDevices().FirstOrDefault(x => x.Id == id || String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case JavaScriptDeviceType.CAPTURE:
                    device = AudioController.GetCaptureDevices().FirstOrDefault(x => x.Id == id || String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    break;
            }

            return device != null ? CreateJavaScriptAudioDevice(device) : null;
        }

        /// <summary>
        ///     Returns the default device for the flag
        /// </summary>
        /// <param name="type">PLAYBACK, CAPTURE</param>
        /// <returns></returns>
        [JSFunction(Name = "getDefaultDevice")]
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
        [JSFunction(Name = "getDefaultCommunicationDevice")]
        public JavaScriptAudioDevice GetDefaultCommunicationDevice(string type)
        {
            switch (type)
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