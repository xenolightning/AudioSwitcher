using System;
using AudioSwitcher.Scripting;

namespace AudioSwitcher.CLI
{
    public sealed class ConsoleScriptOutput : IScriptOutput
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message, Exception ex = null)
        {
            Console.Error.WriteLine(message);

            if(ex != null)
                Console.Error.WriteLine(ex.ToString());
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
