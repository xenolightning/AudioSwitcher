using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary : ObjectInstance, IJavaScriptLibrary
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

        public IAudioController AudioController
        {
            get;
            private set;
        }

        public string Name
        {
            get { return "AudioSwitcher"; }
        }

        public int Version
        {
            get { return 1; }
        }

        public void Add(ScriptEngine engine)
        {
            if (engine.GetGlobalValue(Name) == Undefined.Value)
                engine.SetGlobalValue(Name, this);
        }

        public void Remove(ScriptEngine engine)
        {
            if (engine.GetGlobalValue(Name) != Undefined.Value)
                engine.Global.Delete(Name, false);
        }

        public JavaScriptAudioDevice CreateJavaScriptAudioDevice(IDevice audioDevice)
        {
            if (audioDevice == null)
                return null;

            return new JavaScriptAudioDevice(Engine, AudioController, audioDevice);
        }
    }
}