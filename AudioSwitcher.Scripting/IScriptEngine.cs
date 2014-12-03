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

        IScriptInfo ScriptInfo
        {
            get;
        }

        void SetOutput(IScriptOutput output);

        ExecutionResult Execute(string script);

        Task<ExecutionResult> ExecuteAsync(string script);

        ExecutionResult Execute(IScriptSource scriptSource);

        Task<ExecutionResult> ExecuteAsync(IScriptSource scriptSource);

        TReturn Evaluate<TReturn>(string script);

        Task<TReturn> EvaluateAsync<TReturn>(string script);

        TReturn Evaluate<TReturn>(IScriptSource scriptSource);

        Task<TReturn> EvaluateAsync<TReturn>(IScriptSource scriptSource);

    }
}