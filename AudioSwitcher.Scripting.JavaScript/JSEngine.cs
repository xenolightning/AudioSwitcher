using System;
using System.Collections.Generic;
using System.Threading;

namespace AudioSwitcher.Scripting.JavaScript
{
    public sealed class JsEngine : ScriptEngineBase
    {

        public override IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            var ctx = new JsExecutionContext(isDebug, args, cancellationToken);
            foreach (var lib in ScriptLibraries)
            {
                ctx.AddLibrary(lib.Key, lib.Value);
            }

            ContextCreationAction(ctx);

            return ctx;
        }

    }
}