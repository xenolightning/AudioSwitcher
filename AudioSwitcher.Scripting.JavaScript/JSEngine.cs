using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.Scripting.JavaScript.Internal;
using Jint;
using Jint.Runtime;

namespace AudioSwitcher.Scripting.JavaScript
{
    public sealed class JsEngine : ScriptEngineBase
    {
        private readonly Dictionary<string, IScriptLibrary> _libraryDictionary;
        private bool _isDebug;

        public Engine InternalEngine
        {
            get;
            private set;
        }

        public override string FriendlyName
        {
            get { return "JavaScript Engine"; }
        }

        public override bool IsDebug
        {
            get
            {
                return _isDebug;
            }
            set
            {
                _isDebug = value;
                InternalEngine.SetValue("isDebug", value);
            }
        }

        public JsEngine()
        {
            _libraryDictionary = new Dictionary<string, IScriptLibrary>();

            InternalEngine = new Engine(cfg =>
            {
                cfg.AllowClr();
            });

            InternalEngine.SetValue("require", new Func<string, object>(ImportLibrary));
            SetOutput(new NullScriptOutput());
        }

        private object ImportLibrary(string libraryName)
        {
            if (!_libraryDictionary.ContainsKey(libraryName))
                return null;

            return _libraryDictionary[libraryName];
        }

        public override void SetOutput(IScriptOutput output)
        {
            //var console = new FirebugConsole(InternalEngine)
            //{
            //    Output = new ScriptOutputProxy(output)
            //};

            InternalEngine.SetValue("console", new ScriptOutputProxy(output));
        }

        public override void AddLibrary(string name, IScriptLibrary libraryInstance)
        {
            if (_libraryDictionary.ContainsKey(name))
                return;

            _libraryDictionary.Add(name, libraryInstance);
        }

        public override void AddLibrary(string name, Func<IScriptEngine, IScriptLibrary> libraryInstance)
        {
            AddLibrary(name, libraryInstance(this));
        }

        public override ExecutionResult Execute(IScriptSource scriptSource, IEnumerable<string> args = null)
        {
            try
            {
                if (args != null)
                    InternalEngine.SetValue("args", args.ToArray());

                using (var reader = scriptSource.GetReader())
                {
                    InternalEngine.Execute(reader.ReadToEnd());
                }

                return new ExecutionResult
                {
                    Success = true
                };
            }
            catch (JavaScriptException ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ExecutionException = new ExecutionException(ex)
                    {
                        LineNumber = ex.LineNumber
                    }
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ExecutionException = new ExecutionException(ex)
                };
            }
        }


        public override ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource, IEnumerable<string> args = null)
        {
            try
            {
                if (args != null)
                    InternalEngine.SetValue("args", args.ToArray());

                TReturn val;

                if (typeof(TReturn).IsArray)
                    val = EvaluateArray<TReturn>(scriptSource);
                else if (typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)) && typeof(TReturn) != typeof(string))
                    val = EvaluateEnumerable<TReturn>(scriptSource);
                else
                    val = (TReturn)Convert.ChangeType(InternalEngine.Execute(scriptSource.GetReader().ReadToEnd()).GetCompletionValue().ToObject(), typeof(TReturn));

                return new ExecutionResult<TReturn>
                {
                    Value = val,
                    Success = true
                };
            }
            catch (JavaScriptException ex)
            {
                return new ExecutionResult<TReturn>
                {
                    Success = false,
                    ExecutionException = new ExecutionException(ex)
                    {
                        LineNumber = ex.LineNumber
                    }
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<TReturn>
                {
                    Success = false,
                    ExecutionException = new ExecutionException(ex)
                };
            }
        }

        private TReturn EvaluateArray<TReturn>(IScriptSource scriptSource)
        {
            if (!typeof(TReturn).IsArray)
                return default(TReturn);

            var castMethod = typeof(Enumerable).GetMethod("Cast");
            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray");

            var returnType = typeof(TReturn);
            var obj = InternalEngine.Execute(scriptSource.GetReader().ReadToEnd()).GetCompletionValue().ToObject();


            var targetType = returnType.GetElementType();
            var cast = castMethod.MakeGenericMethod(targetType).Invoke(null, new[] { obj });
            return (TReturn)toArrayMethod.MakeGenericMethod(targetType).Invoke(null, new[] { cast });
        }

        private TReturn EvaluateEnumerable<TReturn>(IScriptSource scriptSource)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)))
                return default(TReturn);

            var castMethod = typeof(Enumerable).GetMethod("Cast");
            var toListMethod = typeof(Enumerable).GetMethod("ToList");

            var returnType = typeof(TReturn);
            var obj = InternalEngine.Execute(scriptSource.GetReader().ReadToEnd()).GetCompletionValue().ToObject();

            var targetType = returnType.GetGenericArguments()[0];
            var cast = castMethod.MakeGenericMethod(targetType).Invoke(null, new[] { obj });
            return (TReturn)toListMethod.MakeGenericMethod(targetType).Invoke(null, new[] { cast });
        }


        protected override void Dispose(bool disposing)
        {
            InternalEngine = null;
        }
    }
}