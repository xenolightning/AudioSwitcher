using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary : IScriptLibrary
    {
        private readonly JavaScriptDeviceType _deviceType;
        private readonly JavaScriptDeviceState _deviceState;

        public AudioSwitcherLibrary(IAudioController controller)
        {
            AudioController = controller;
            _deviceType = new JavaScriptDeviceType();
            _deviceState = new JavaScriptDeviceState();
        }

        private IAudioController AudioController
        {
            get; 
            set;
        }

        public JavaScriptAudioDevice CreateJavaScriptAudioDevice(IDevice audioDevice)
        {
            if (audioDevice == null)
                return null;

            return new JavaScriptAudioDevice(AudioController, audioDevice);
        }
    }
}