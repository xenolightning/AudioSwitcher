using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class AudioSwitcherLibrary : ObjectInstance, IJavaScriptLibrary
    {
        public AudioSwitcherLibrary(ScriptEngine engine, AudioController controller)
            : base(engine)
        {
            AudioController = controller;
            PopulateFields();
            PopulateFunctions();
        }

        public AudioController AudioController
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
            return new JavaScriptAudioDevice(Engine, AudioController, audioDevice);
        }
    }
}