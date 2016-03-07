using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MoonSharp.Interpreter;

namespace AudioSwitcher.Scripting.Lua
{
    public sealed class LuaEngine : ScriptEngineBase
    {

        public LuaEngine()
            : base()
        {
            UserData.RegisterAssembly();
        }

        protected override IExecutionContext GetNewContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            return new LuaExecutionContext(isDebug, args, cancellationToken);
        }
    }
}
