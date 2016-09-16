using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.Scripting.Lua
{
    public sealed class LuaExecutionContext : IExecutionContext
    {
        private readonly bool _isDebug;
        private CancellationToken _cancellationToken;
        private readonly Dictionary<string, object> _libraries;

        public LuaExecutionContext(bool isDebug, CancellationToken cancellationToken)
            : this(isDebug, Enumerable.Empty<string>(), cancellationToken)
        {

        }

        public LuaExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            _isDebug = isDebug;
            _cancellationToken = cancellationToken;
            _libraries = new Dictionary<string, object>();

            //_engine = new Engine(cfg =>
            //{
            //    cfg.AllowClr();

            //    //Set cancellation token
            //});

            //var lArgs = args?.ToArray() ?? new string[] { };
            //if (lArgs.Any())
            //    _engine.SetValue("args", lArgs);

            //_engine.SetValue("require", new Func<string, object>(ImportLibrary));
            //SetOutput(new NullScriptOutput());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsDebug
        {
            get
            {
                return _isDebug;
            }
        }

        public IDictionary<string, object> Libraries => new ReadOnlyDictionary<string, object>(_libraries);

        public void SetOutput(IScriptOutput output)
        {
            throw new NotImplementedException();
        }

        public void AddLibrary(string name, IScriptLibrary libraryInstance)
        {
            throw new NotImplementedException();
        }

        public void AddLibrary(string name, Func<IExecutionContext, IScriptLibrary> libraryInstance)
        {
            throw new NotImplementedException();
        }

        public object Resolve(string name)
        {
            throw new NotImplementedException();
        }

        public ExecutionResult Execute(string script, IEnumerable<string> args = null)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult> ExecuteAsync(string script, IEnumerable<string> args = null)
        {
            throw new NotImplementedException();
        }

        public ExecutionResult<TReturn> Evaluate<TReturn>(string script, IEnumerable<string> args = null)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<TReturn>> EvaluateAsync<TReturn>(string script, IEnumerable<string> args = null)
        {
            throw new NotImplementedException();
        }
    }
}