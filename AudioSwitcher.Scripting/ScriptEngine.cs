using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public abstract class ScriptEngine<T> : IScriptEngine<T>
        where T : IScript
    {

        public abstract string FriendlyName
        {
            get;
        }

        public abstract IScriptInfo ScriptInfo
        {
            get;
        }

        public abstract void SetOutput(IScriptOutput output);

        public ExecutionResult Execute(string script)
        {
            return Execute(new StringScriptSource(script));
        }

        public Task<ExecutionResult> ExecuteAsync(string script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public abstract ExecutionResult Execute(IScriptSource scriptSource);

        public Task<ExecutionResult> ExecuteAsync(IScriptSource scriptSource)
        {
            return Task.Factory.StartNew(() => Execute(scriptSource));
        }

        public ExecutionResult<TReturn> Evaluate<TReturn>(string script)
        {
            return this.Evaluate<TReturn>(new StringScriptSource(script));
        }

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script)
        {
            return Task.Factory.StartNew(() => this.Evaluate<TReturn>(script));
        }

        public abstract ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource);

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(IScriptSource scriptSource)
        {
            return Task.Factory.StartNew(() => Evaluate<TReturn>(scriptSource));
        }

        public abstract ExecutionResult Execute(T script);

        public Task<ExecutionResult> ExecuteAsync(T script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public ExecutionResult<TReturn> Evaluate<TReturn>(T script)
        {
            return Evaluate<TReturn>(script.Source);
        }

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(T script)
        {
            return Task.Factory.StartNew(() => Evaluate<TReturn>(script));
        }

        public abstract T NewScript();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

    }
}
