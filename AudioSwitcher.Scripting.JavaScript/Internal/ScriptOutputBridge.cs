using System;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{

    /// <summary>
    /// Represents an implementation of the Firebug API using the standard console.
    /// </summary>
    internal class ScriptOutputBridge : IFirebugConsoleOutput
    {
        private readonly IScriptOutput _output;

        public ScriptOutputBridge(IScriptOutput output)
        {
            _output = output;
        }

        public void Log(FirebugConsoleMessageStyle style, object[] objects)
        {
            // Convert the objects to a string.
            var message = new System.Text.StringBuilder();
            for (int i = 0; i < objects.Length; i++)
            {
                message.Append(' ');
                message.Append(TypeConverter.ToString(objects[i]));
            }

            switch (style)
            {
                case FirebugConsoleMessageStyle.Regular:
                case FirebugConsoleMessageStyle.Information:
                case FirebugConsoleMessageStyle.Warning:
                    _output.Log(message.ToString());
                    break;
                case FirebugConsoleMessageStyle.Error:
                    _output.Error(message.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("style");
            }
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