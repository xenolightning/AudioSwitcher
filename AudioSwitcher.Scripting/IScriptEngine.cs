using System;
using System.Collections.Generic;
using System.Threading;

namespace AudioSwitcher.Scripting
{
    public interface IScriptEngine
    {
        /// <summary>
        /// Adds this library to each execution context that is created from this engine
        /// </summary>
        /// <param name="name"></param>
        /// <param name="libraryFunc"></param>
        void AddLibrary(string name, Func<IExecutionContext, IScriptLibrary> libraryFunc);

        IExecutionContext CreateExecutionContext();

        IExecutionContext CreateExecutionContext(IEnumerable<string> args);

        IExecutionContext CreateExecutionContext(bool isDebug);

        IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args);

        IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken);

        void OnExecutionContextCreation(Action<IExecutionContext> contextCreationAction);

    }
}