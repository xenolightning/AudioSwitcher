using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal.Libraries;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public static class ScriptEngineExtensions
    {

        public static void AddAudioSwitcherLibrary(this IExecutionContext context, IAudioController controller)
        {
            if (context is JsExecutionContext)
                context.AddLibrary("AudioSwitcher", new AudioSwitcherLibrary(controller));
        }

        public static void AddCoreLibrary(this IExecutionContext context)
        {
            if (context is JsExecutionContext)
                context.AddLibrary("Core", new CoreLibrary());
        }

    }
}