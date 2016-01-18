
using System;
using System.Threading;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class CoreLibrary : IScriptLibrary
    {

        public void Sleep(int s)
        {
            Thread.Sleep(s);
        }

        public string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}