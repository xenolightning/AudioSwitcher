using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AudioSwitcher.Scripting.JavaScript.Internal;
using Jint;
using Jint.Runtime;

namespace AudioSwitcher.Scripting.JavaScript
{
    public sealed class JsEngine : IScriptEngine
    {

        public string FriendlyName
        {
            get { return "JavaScript Engine"; }
        }

        public IExecutionContext CreateExecutionContext()
        {
            return CreateExecutionContext(false);
        }

        public IExecutionContext CreateExecutionContext(IEnumerable<string> args)
        {
            return CreateExecutionContext(false, args);
        }

        public IExecutionContext CreateExecutionContext(bool isDebug)
        {
            return CreateExecutionContext(isDebug, null);
        }

        public IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args)
        {
            return CreateExecutionContext(isDebug, args, CancellationToken.None);
        }

        public IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            return new JsExecutionContext(isDebug, args, cancellationToken);
        }

    }
}