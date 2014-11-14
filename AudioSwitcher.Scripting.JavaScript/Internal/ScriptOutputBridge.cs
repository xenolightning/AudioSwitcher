using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public class ScriptOutputBridge : IFirebugConsoleOutput
    {
        private readonly IScriptOutput _output;

        public ScriptOutputBridge(IScriptOutput output)
        {
            _output = output;
        }

        public void Log(FirebugConsoleMessageStyle style, object[] objects)
        {
        }

        public void Clear()
        {
        }

        public void StartGroup(string title, bool initiallyCollapsed)
        {
        }

        public void EndGroup()
        {
        }
    }
}
