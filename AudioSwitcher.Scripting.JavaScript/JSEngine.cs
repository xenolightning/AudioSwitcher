using System;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSEngine : IScriptEngine<JSScript>
    {
        public ScriptEngine InternalEngine
        {
            get;
            private set;
        }

        public JSEngine(AudioController controller)
        {
            InternalEngine = new ScriptEngine();
            InternalEngine.AddCoreLibrary();
            InternalEngine.AddAudioSwitcherLibrary(controller);
        }

        public string FriendlyName
        {
            get { return "JavaScript Engine"; }
        }

        public IScriptInfo ScriptInfo
        {
            get { return JSScriptInfo.Instance; }
        }

        public void SetOutput(IScriptOutput output)
        {
            var console = new FirebugConsole(InternalEngine);
            console.Output = new ScriptOutputBridge(output);
            InternalEngine.SetGlobalValue("console", console);
        }

        public ExecutionResult<string> Execute(string script)
        {
            try
            {
                InternalEngine.Execute(script);
                return new ExecutionResult<string>
                {
                    Script = script,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<string>
                {
                    Script = script,
                    Success = false,
                    ExecutionException = ex
                };
            }
        }

        public Task<ExecutionResult<string>> ExecuteAsync(string script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public TReturn Evaluate<TReturn>(string script)
        {
            return InternalEngine.Evaluate<TReturn>(script);
        }

        public Task<TReturn> EvaluateAsync<TReturn>(string script)
        {
            return Task.Factory.StartNew(() => InternalEngine.Evaluate<TReturn>(script));
        }


        public ExecutionResult<JSScript> Execute(JSScript script)
        {
            try
            {
                InternalEngine.Execute(script.Content);
                return new ExecutionResult<JSScript>
                {
                    Script = script,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<JSScript>
                {
                    Script = script,
                    Success = false,
                    ExecutionException = ex
                };
            }
        }

        public Task<ExecutionResult<JSScript>> ExecuteAsync(JSScript script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public TReturn Evaluate<TReturn>(JSScript script)
        {
            return InternalEngine.Evaluate<TReturn>(script.Content);
        }

        public Task<TReturn> EvaluateAsync<TReturn>(JSScript script)
        {
            return Task.Factory.StartNew(() => InternalEngine.Evaluate<TReturn>(script.Content));
        }

        public JSScript NewScript()
        {
            return new JSScript();
        }

        public void Dispose()
        {
            InternalEngine = null;
        }
    }
}