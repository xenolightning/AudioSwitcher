
using System.Threading;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class CoreLibrary : IScriptLibrary
    {
        public CoreLibrary()
        {
        }
        public void Sleep(int s)
        {
            Thread.Sleep(s);
        }
    }
}