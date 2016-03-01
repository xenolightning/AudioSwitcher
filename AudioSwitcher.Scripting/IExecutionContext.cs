using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public interface IExecutionContext : IDisposable
    {
        bool IsDebug { get; }

        IDictionary<string, object> Libraries { get; }

        void SetOutput(IScriptOutput output);

        void AddLibrary(string name, IScriptLibrary libraryInstance);

        void AddLibrary(string name, Func<IExecutionContext, IScriptLibrary> libraryInstance);

        object Resolve(string name);

        ExecutionResult Execute(string script, IEnumerable<string> args = null);

        Task<ExecutionResult> ExecuteAsync(string script, IEnumerable<string> args = null);

        ExecutionResult<TReturn> Evaluate<TReturn>(string script, IEnumerable<string> args = null);

        Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script, IEnumerable<string> args = null);

    }
}