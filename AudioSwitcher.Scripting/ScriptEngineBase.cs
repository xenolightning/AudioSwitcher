using System;
using System.Collections.Generic;
using System.Threading;

namespace AudioSwitcher.Scripting
{
    public abstract class ScriptEngineBase : IScriptEngine
    {
        protected readonly Dictionary<string, Func<IExecutionContext, IScriptLibrary>> ScriptLibraries;
        protected Action<IExecutionContext> ContextCreationAction;

        public string FriendlyName => "JavaScript Engine";

        public ScriptEngineBase()
        {
            ScriptLibraries = new Dictionary<string, Func<IExecutionContext, IScriptLibrary>>();
            ContextCreationAction = x => { };
        }

        public void AddLibrary(string name, Func<IExecutionContext, IScriptLibrary> libraryFunc)
        {
            if (ScriptLibraries.ContainsKey(name))
                return;

            ScriptLibraries.Add(name, libraryFunc);
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

        public abstract IExecutionContext CreateExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken);

        public void OnExecutionContextCreation(Action<IExecutionContext> contextCreationAction)
        {
            ContextCreationAction = contextCreationAction;
        }
    }
}