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

        ExecutionResult<string> Execute(string script);

        Task<ExecutionResult<string>> ExecuteAsync(string script);

        TReturn Evaluate<TReturn>(string script);

        Task<TReturn> EvaluateAsync<TReturn>(string script);

    }
}