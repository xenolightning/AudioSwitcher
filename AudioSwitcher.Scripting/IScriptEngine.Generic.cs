using System.IO;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public interface IScriptEngine<T> : IScriptEngine
        where T : IScript
    {

        ExecutionResult<T> Execute(T script);

        Task<ExecutionResult<T>> ExecuteAsync(T script);

        TReturn Evaluate<TReturn>(T script);

        Task<TReturn> EvaluateAsync<TReturn>(T script);

        T NewScript();
    }
}