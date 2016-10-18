using System.Collections.Generic;
using System.Threading;

namespace AudioSwitcher.Scripting.JavaScript
{
    public sealed class JsEngine : ScriptEngineBase
    {

        protected override IExecutionContext GetNewContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            return new JsExecutionContext(isDebug, args, cancellationToken);
        }

    }
}