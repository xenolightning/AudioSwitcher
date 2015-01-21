using System;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSEngine : ScriptEngine<JSScript>
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

        public override string FriendlyName
        {
            get { return "JavaScript Engine"; }
        }

        public override IScriptInfo ScriptInfo
        {
            get { return JSScriptInfo.Instance; }
        }

        public override void SetOutput(IScriptOutput output)
        {
            var console = new FirebugConsole(InternalEngine);
            console.Output = new ScriptOutputProxy(output);
            InternalEngine.SetGlobalValue("console", console);
        }


        public override ExecutionResult Execute(IScriptSource scriptSource)
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


        public override TReturn Evaluate<TReturn>(IScriptSource scriptSource)
        {
            return InternalEngine.Evaluate<TReturn>(scriptSource.GetReader().ReadToEnd());
        }

        public override ExecutionResult Execute(JSScript script)
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

        public override JSScript NewScript()
        {
            return new JSScript();
        }

        protected override void Dispose(bool disposing)
        {
            InternalEngine = null;
        }
    }
}