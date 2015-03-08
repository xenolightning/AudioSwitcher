using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary : ObjectInstance, IScriptLibrary
    {
        private readonly JavaScriptDeviceType _deviceType;
        private readonly JavaScriptDeviceState _deviceState;

        public AudioSwitcherLibrary(ScriptEngine engine, IAudioController controller)
            : base(engine)
        {
            AudioController = controller;
            _deviceType = new JavaScriptDeviceType(engine);
            _deviceState = new JavaScriptDeviceState(engine);

            PopulateFields();
            PopulateFunctions();

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

            return new JavaScriptAudioDevice(Engine, AudioController, audioDevice);
        }
    }
}