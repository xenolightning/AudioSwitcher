using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public interface IScriptEngine
    {
        string FriendlyName
        {
            get;
        }

        IExecutionContext CreateExecutionContext();

        IExecutionContext CreateExecutionContext(IEnumerable<string> args);

        IExecutionContext CreateExecutionContext(bool isDebug);

        IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args);

        IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken);

    }
}