using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public interface IScriptEngine : IDisposable
    {
        string FriendlyName
        {
            get;
        }

        void SetOutput(IScriptOutput output);

        void AddLibrary(string name, IScriptLibrary libraryInstance);
        void AddLibrary(string name, Func<IScriptEngine, IScriptLibrary> libraryInstance);

        ExecutionResult Execute(string script, IEnumerable<string> args = null);

        Task<ExecutionResult> ExecuteAsync(string script, IEnumerable<string> args = null);

        ExecutionResult Execute(IScriptSource scriptSource, IEnumerable<string> args = null);

        Task<ExecutionResult> ExecuteAsync(IScriptSource scriptSource, IEnumerable<string> args = null);

        ExecutionResult<TReturn> Evaluate<TReturn>(string script, IEnumerable<string> args = null);

        Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script, IEnumerable<string> args = null);

        ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource, IEnumerable<string> args = null);

        Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(IScriptSource scriptSource, IEnumerable<string> args = null);

    }
}