using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public interface IScriptEngine<T> : IScriptEngine
        where T : IScript
    {

        ExecutionResult Execute(T script);

        Task<ExecutionResult> ExecuteAsync(T script);

        TReturn Evaluate<TReturn>(T script);

        Task<TReturn> EvaluateAsync<TReturn>(T script);

        T NewScript();
    }
}