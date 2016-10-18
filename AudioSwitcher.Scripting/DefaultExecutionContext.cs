using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public abstract class DefaultExecutionContext : IExecutionContext
    {

        public abstract bool IsDebug { get; }

        public abstract IDictionary<string, object> Libraries { get; }

        public abstract void SetOutput(IScriptOutput output);

        public abstract void AddLibrary(string name, IScriptLibrary libraryInstance);

        public abstract void AddLibrary(string name, Func<IExecutionContext, IScriptLibrary> libraryInstance);

        public abstract object Resolve(string name);

        public Task<ExecutionResult> ExecuteAsync(string script, IEnumerable<string> args = null)
        {
            return Task.Run(() => Execute(script, args));
        }

        public abstract ExecutionResult Execute(string script, IEnumerable<string> args = null);

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script, IEnumerable<string> args = null)
        {
            return Task.Run(() => Evaluate<TReturn>(script, args));
        }

        public abstract ExecutionResult<TReturn> Evaluate<TReturn>(string script, IEnumerable<string> args = null);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}
