using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting
{
    public interface IScriptEngine<T> : IDisposable
        where T : IScript
    {

        string FriendlyName { get; }

        IScriptInfo ScriptInfo { get; }

        ExecutionResult Execute(T scriptScript);

        Task<ExecutionResult> ExecuteAsync(T scriptScript);

        T NewScript();

    }
}
