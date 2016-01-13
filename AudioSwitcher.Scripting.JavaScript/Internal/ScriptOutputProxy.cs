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

        public void Log(JsValue[] objects)
        {
            // Convert the objects to a string.
            var message = new StringBuilder();
            foreach (var t in objects)
            {
                message.Append(' ');
                message.Append(t.AsString());
            }

            _output.Log(message.ToString());
        }

        public void Error(JsValue[] objects)
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

        public void Warn(JsValue[] objects)
        {
            Log(objects);
        }

        public void Clear()
        {
            _output.Clear();
        }

        public void StartGroup(string title, bool initiallyCollapsed)
        {
        }

        public void EndGroup()
        {
        }
    }
}