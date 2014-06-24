using System.Threading;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.Libraries
{
    public sealed partial class CoreLibrary
    {
        [JSFunction(Name = "sleep")]
        public void Sleep(int s)
        {
            Thread.Sleep(s);
        }
    }
}