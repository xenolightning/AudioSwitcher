using System;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using Jurassic;

namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSEngine : IScriptEngine<JSScript>
    {
        private ScriptEngine _engine;

        public JSEngine(AudioController controller)
        {
            _engine = new ScriptEngine();
            _engine.AddCoreLibrary();
            _engine.AddAudioSwitcherLibrary(controller);
        }

        public string SyntaxHighlightingCode
        {
            get { return "JavaScript"; }
        }

        public string FriendlyName
        {
            get { return "JavaScript Engine"; }
        }

        public IScriptInfo ScriptInfo
        {
            get { return JSScriptInfo.Instance; }
        }


        public ExecutionResult Execute(JSScript script)
        {
            try
            {
                _engine.Execute(script.Content);
                return new ExecutionResult
                {
                    Script = script,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Script = script,
                    Success = false,
                    ExecutionException = ex
                };
            }
        }

        public Task<ExecutionResult> ExecuteAsync(JSScript script)
        {
            return Task.Factory.StartNew(() => Execute(script));
        }

        public JSScript NewScript()
        {
            return new JSScript();
        }

        public void Dispose()
        {
            _engine = null;
        }
    }
}