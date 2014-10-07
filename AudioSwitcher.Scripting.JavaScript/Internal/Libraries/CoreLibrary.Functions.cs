using System.Threading;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class CoreLibrary
    {
        [JSFunction(Name = "sleep")]
        public void Sleep(int s)
        {
            Thread.Sleep(s);
        }
    }
}