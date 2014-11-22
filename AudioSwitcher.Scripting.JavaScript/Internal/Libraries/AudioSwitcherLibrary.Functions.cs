using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary
    {
        /// <summary>
        ///     Macro function used to list all the devices
        /// </summary>
        /// <param name="flags">0 Both, 1 = Playback, 2 = Capture</param>
        /// <returns></returns>
        [JSFunction(Name = "getAudioDevices")]
        public ArrayInstance GetAudioDevices([DefaultParameterValue(0)] int flags = 0)
        {
            var devices = new List<IDevice>();

            switch (flags)
            {
                case 0:
                    devices.AddRange(AudioController.GetPlaybackDevices());
                    devices.AddRange(AudioController.GetCaptureDevices());
                    break;
                case 1:
                    devices.AddRange(AudioController.GetPlaybackDevices());
                    break;
                case 2:
                    devices.AddRange(AudioController.GetCaptureDevices());
                    break;
            }

            //if empty then return empty array
            if (devices.Count == 0)
                return Engine.Array.New();

            return Engine.EnumerableToArray(devices.Select(CreateJavaScriptAudioDevice));
        }

        /// <summary>
        ///     Get an audio device by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [JSFunction(Name = "getAudioDevice")]
        public JavaScriptAudioDevice GetAudioDevice(string name, [DefaultParameterValue(0)] int flags = 0)
        {
            var devices = new List<IDevice>();

            switch (flags)
            {
                case 0:
                    devices.AddRange(AudioController.GetPlaybackDevices());
                    devices.AddRange(AudioController.GetCaptureDevices());
                    break;
                case 1:
                    devices.AddRange(AudioController.GetPlaybackDevices());
                    break;
                case 2:
                    devices.AddRange(AudioController.GetCaptureDevices());
                    break;
            }

            IDevice dev = devices.FirstOrDefault(x => x.Name == name);

            return dev != null ? CreateJavaScriptAudioDevice(dev) : null;
        }

        /// <summary>
        ///     Returns the default device for the flag
        /// </summary>
        /// <param name="flags">1 = Playback, 2 = Capture</param>
        /// <returns></returns>
        [JSFunction(Name = "getDefaultDevice")]
        public JavaScriptAudioDevice GetDefaultDevice(int flags)
        {
            switch (flags)
            {
                case 1:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultPlaybackDevice);
                case 2:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultCaptureDevice);
            }

            return null;
        }

        /// <summary>
        ///     Returns the default communication device for the flag
        /// </summary>
        /// <param name="flags">1 = Playback, 2 = Capture</param>
        /// <returns></returns>
        [JSFunction(Name = "getDefaultCommunicationDevice")]
        public JavaScriptAudioDevice GetDefaultCommunicationDevice(int flags)
        {
            switch (flags)
            {
                case 1:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultPlaybackCommunicationsDevice);
                case 2:
                    return CreateJavaScriptAudioDevice(AudioController.DefaultCaptureCommunicationsDevice);
            }

            return null;
        }
    }
}