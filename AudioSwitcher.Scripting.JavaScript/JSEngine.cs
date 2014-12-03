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
            console.Output = new ScriptOutputProxy(output);
            InternalEngine.SetGlobalValue("console", console);
        }

        public ExecutionResult Execute(string script)
        {
            return Execute(new StringScriptSource(script));
        }

        public Task<ExecutionResult> ExecuteAsync(string script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public ExecutionResult Execute(IScriptSource scriptSource)
        {
            try
            {
                InternalEngine.Execute(new ScriptSourceProxy(scriptSource));
                return new ExecutionResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ExecutionException = ex
                };
            }
        }

        public Task<ExecutionResult> ExecuteAsync(IScriptSource scriptSource)
        {
            return Task.Factory.StartNew(() => Execute(scriptSource));
        }

        public TReturn Evaluate<TReturn>(string script)
        {
            return InternalEngine.Evaluate<TReturn>(script);
        }

        public Task<TReturn> EvaluateAsync<TReturn>(string script)
        {
            return Task.Factory.StartNew(() => InternalEngine.Evaluate<TReturn>(script));
        }

        public TReturn Evaluate<TReturn>(IScriptSource scriptSource)
        {
            return InternalEngine.Evaluate<TReturn>(new ScriptSourceProxy(scriptSource));
        }

        public Task<TReturn> EvaluateAsync<TReturn>(IScriptSource scriptSource)
        {
            return Task.Factory.StartNew(() => Evaluate<TReturn>(scriptSource));
        }


        public ExecutionResult Execute(JSScript script)
        {
            try
            {
                InternalEngine.Execute(new ScriptSourceProxy(script.Source));
                return new ExecutionResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ExecutionException = ex
                };
            }
        }

        public Task<ExecutionResult> ExecuteAsync(JSScript script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public TReturn Evaluate<TReturn>(JSScript script)
        {
            return Evaluate<TReturn>(script.Source);
        }

        public Task<TReturn> EvaluateAsync<TReturn>(JSScript script)
        {
            return Task.Factory.StartNew(() => Evaluate<TReturn>(script));
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