using System;
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

        ExecutionResult Execute(string script);

        Task<ExecutionResult> ExecuteAsync(string script);

        ExecutionResult Execute(IScriptSource scriptSource);

        Task<ExecutionResult> ExecuteAsync(IScriptSource scriptSource);

        ExecutionResult<TReturn> Evaluate<TReturn>(string script);

        Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script);

        ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource);

        Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(IScriptSource scriptSource);

    }
}