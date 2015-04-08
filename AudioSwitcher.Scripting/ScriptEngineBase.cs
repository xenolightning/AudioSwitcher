using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public abstract class ScriptEngineBase : IScriptEngine
    {

        public abstract string FriendlyName
        {
            get;
        }

        public abstract bool IsDebug { get; set; }

        public abstract void SetOutput(IScriptOutput output);

        public abstract void AddLibrary(string name, IScriptLibrary libraryInstance);
        public abstract void AddLibrary(string name, Func<IScriptEngine, IScriptLibrary> libraryInstance);

        public ExecutionResult Execute(string script, IEnumerable<string> args = null)
        {
            return Execute(new StringScriptSource(script));
        }

        public Task<ExecutionResult> ExecuteAsync(string script, IEnumerable<string> args = null)
        {
            return Task.Factory.StartNew(() => Execute(script, args));
        }

        public abstract ExecutionResult Execute(IScriptSource scriptSource, IEnumerable<string> args = null);

        public Task<ExecutionResult> ExecuteAsync(IScriptSource scriptSource, IEnumerable<string> args = null)
        {
            return Task.Factory.StartNew(() => Execute(scriptSource, args));
        }

        public ExecutionResult<TReturn> Evaluate<TReturn>(string script, IEnumerable<string> args = null)
        {
            return Evaluate<TReturn>(new StringScriptSource(script), args);
        }

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script, IEnumerable<string> args = null)
        {
            return Task.Factory.StartNew(() => Evaluate<TReturn>(script, args));
        }

        public abstract ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource, IEnumerable<string> args = null);

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(IScriptSource scriptSource, IEnumerable<string> args = null)
        {
            return Task.Factory.StartNew(() => Evaluate<TReturn>(scriptSource, args));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

    }
}
