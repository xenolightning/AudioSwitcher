using System.Text;
using Jint.Native;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{

    /// <summary>
    /// Represents an implementation of the Firebug API using the standard console.
    /// </summary>
    internal class ScriptOutputProxy
    {
        private readonly IScriptOutput _output;

        public ScriptOutputProxy(IScriptOutput output)
        {
            _output = output;
        }

        public void Log(params JsValue[] objects)
        {
            // Convert the objects to a string.
            var message = new StringBuilder();
            foreach (var t in objects)
            {
                message.Append(' ');
                message.Append(t);
            }

            _output.Log(message.ToString());
        }

        public void Error(params JsValue[] objects)
        {
            // Convert the objects to a string.
            var message = new StringBuilder();
            foreach (var t in objects)
            {
                message.Append(' ');
                message.Append(t.AsString());
            }

            _output.Error(message.ToString());
        }

        public void Warn(params JsValue[] objects)
        {
            Log(objects);
        }

        public void Clear()
        {
            _output.Clear();
        }

    }
}