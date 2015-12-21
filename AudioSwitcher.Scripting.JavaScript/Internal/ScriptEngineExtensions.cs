using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal.Libraries;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public static class ScriptEngineExtensions
    {

        public static void AddAudioSwitcherLibrary(this JsEngine engine, IAudioController controller)
        {
            engine.AddLibrary("AudioSwitcher", new AudioSwitcherLibrary(engine.InternalEngine, controller));
        }
        public static void AddCoreLibrary(this JsEngine engine)
        {
            engine.AddLibrary("Core", new CoreLibrary(engine.InternalEngine));
        }

        public static ArrayInstance EnumerableToArray(this ScriptEngine engine, IEnumerable<object> collection)
        {
            return engine.Array.New(collection.ToArray());
        }

        public static ArrayInstance EnumerableToArray<T>(this ScriptEngine engine, IEnumerable<T> collection)
            where T : class
        {
            return engine.Array.New(collection.Cast<object>().ToArray());
        }

    }
}