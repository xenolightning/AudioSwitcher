using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AudioSwitcher.Scripting.JavaScript.Internal;
using Jint;
using Jint.Native;
using Jint.Runtime;

namespace AudioSwitcher.Scripting.JavaScript
{
    internal sealed class JsExecutionContext : DefaultExecutionContext
    {
        private readonly bool _isDebug;
        private readonly CancellationToken _cancellationToken;
        private readonly Dictionary<string, object> _libraries;
        private readonly Engine _engine;

        public override bool IsDebug => _isDebug;

        public override IDictionary<string, object> Libraries => _libraries;

        internal Engine InternalEngine => _engine;

        public JsExecutionContext(bool isDebug, CancellationToken cancellationToken)
            : this(isDebug, Enumerable.Empty<string>(), cancellationToken)
        {

        }

        public JsExecutionContext(bool isDebug, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            _isDebug = isDebug;
            _cancellationToken = cancellationToken;
            _libraries = new Dictionary<string, object>();

            _engine = new Engine(cfg =>
            {
                cfg.AllowClr();

                //Set cancellation token
            });

            var lArgs = args?.ToArray() ?? new string[] { };
            if (lArgs.Any())
                _engine.SetValue("args", lArgs);

            _engine.SetValue("require", new Func<string, object>(ImportLibrary));
            SetOutput(new NullScriptOutput());
        }

        public override void SetOutput(IScriptOutput output)
        {
            _engine.SetValue("console", new ScriptOutputProxy(output));
        }

        public override void AddLibrary(string name, IScriptLibrary libraryInstance)
        {
            if (_libraries.ContainsKey(name))
                return;

            _libraries.Add(name, libraryInstance);
        }

        public override void AddLibrary(string name, Func<IExecutionContext, IScriptLibrary> libraryInstance)
        {
            AddLibrary(name, libraryInstance(this));
        }

        public override object Resolve(string name)
        {
            var val = _engine.GetValue(name);

            if (val == JsValue.Undefined || val == JsValue.Null)
                return null;

            return val.ToObject();
        }

        public override ExecutionResult Execute(string script, IEnumerable<string> args = null)
        {
            try
            {
                if (args != null)
                    _engine.SetValue("args", args.ToArray());

                _engine.Execute(script);

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


        public override ExecutionResult<TReturn> Evaluate<TReturn>(string script, IEnumerable<string> args = null)
        {
            try
            {
                if (args != null)
                    _engine.SetValue("args", args.ToArray());

                TReturn val;

                if (typeof(TReturn).IsArray)
                    val = EvaluateArray<TReturn>(script);
                else if (typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)) && typeof(TReturn) != typeof(string))
                    val = EvaluateEnumerable<TReturn>(script);
                else
                    val = (TReturn)Convert.ChangeType(_engine.Execute(script).GetCompletionValue().ToObject(), typeof(TReturn));

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

        private TReturn EvaluateArray<TReturn>(string script)
        {
            if (!typeof(TReturn).IsArray)
                return default(TReturn);

            var castMethod = typeof(Enumerable).GetMethod("Cast");
            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray");

            var returnType = typeof(TReturn);
            var obj = _engine.Execute(script).GetCompletionValue().ToObject();


            var targetType = returnType.GetElementType();
            var cast = castMethod.MakeGenericMethod(targetType).Invoke(null, new[] { obj });
            return (TReturn)toArrayMethod.MakeGenericMethod(targetType).Invoke(null, new[] { cast });
        }

        private TReturn EvaluateEnumerable<TReturn>(string script)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)))
                return default(TReturn);

            var castMethod = typeof(Enumerable).GetMethod("Cast");
            var toListMethod = typeof(Enumerable).GetMethod("ToList");

            var returnType = typeof(TReturn);
            var obj = _engine.Execute(script).GetCompletionValue().ToObject();

            var targetType = returnType.GetGenericArguments()[0];
            var cast = castMethod.MakeGenericMethod(targetType).Invoke(null, new[] { obj });
            return (TReturn)toListMethod.MakeGenericMethod(targetType).Invoke(null, new[] { cast });
        }

        private object ImportLibrary(string libraryName)
        {
            if (!_libraries.ContainsKey(libraryName))
                return null;

            return _libraries[libraryName];
        }

    }
}
