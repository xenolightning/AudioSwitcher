using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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


        public override ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource)
        {
            TReturn val = default(TReturn);
            try
            {
                if (typeof(TReturn).IsArray)
                    val = EvaluateArray<TReturn>(scriptSource);
                else if (typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)))
                    val = EvaluateEnumerable<TReturn>(scriptSource);
                else
                    val = InternalEngine.Evaluate<TReturn>(scriptSource.GetReader().ReadToEnd());

                return new ExecutionResult<TReturn> { Result = val, Success = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<TReturn> { Success = false };
            }
        }

        private TReturn EvaluateArray<TReturn>(IScriptSource scriptSource)
        {
            if (!typeof(TReturn).IsArray)
                return default(TReturn);

            MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast");
            MethodInfo toArrayMethod = typeof(Enumerable).GetMethod("ToArray");

            var returnType = typeof(TReturn);
            var obj = InternalEngine.Evaluate<ArrayInstance>(scriptSource.GetReader().ReadToEnd()).ElementValues.ToArray();

            var targetType = returnType.GetElementType();
            var cast = castMethod.MakeGenericMethod(new Type[] { targetType }).Invoke(null, new object[] { obj });
            return (TReturn)toArrayMethod.MakeGenericMethod(new Type[] { targetType }).Invoke(null, new object[] { cast });
        }

        private TReturn EvaluateEnumerable<TReturn>(IScriptSource scriptSource)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)))
                return default(TReturn);

            MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast");
            MethodInfo toListMethod = typeof(Enumerable).GetMethod("ToList");

            var returnType = typeof(TReturn);
            var obj = InternalEngine.Evaluate<ArrayInstance>(scriptSource.GetReader().ReadToEnd()).ElementValues.ToArray();

            var targetType = returnType.GetGenericArguments()[0];
            var cast = castMethod.MakeGenericMethod(new Type[] { targetType }).Invoke(null, new object[] { obj });
            return (TReturn)toListMethod.MakeGenericMethod(new Type[] { targetType }).Invoke(null, new object[] { cast });
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