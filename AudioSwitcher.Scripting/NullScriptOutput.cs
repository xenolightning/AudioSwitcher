using System;

namespace AudioSwitcher.Scripting
{
    /// <summary>
    /// A class to sink log calls in a script when an external output has not been defined
    /// </summary>
    public sealed class NullScriptOutput : IScriptOutput
    {
        public void Log(string message)
        {
        }

        public void Error(string message, Exception ex = null)
        {
        }

        public void Clear()
        {
        }
    }
}
