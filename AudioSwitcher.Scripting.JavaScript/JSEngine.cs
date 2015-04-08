using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jurassic;
using Jurassic.Library;
using AudioSwitcher.Scripting.JavaScript.Internal;

namespace AudioSwitcher.Scripting.JavaScript
{
    public class JsEngine : ScriptEngineBase
    {
        private readonly Dictionary<string, IScriptLibrary> _libraryDictionary;
        private bool _isDebug;

        public ScriptEngine InternalEngine
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
                InternalEngine.SetGlobalValue("isDebug", value);
            }
        }

        public JsEngine()
        {
            _libraryDictionary = new Dictionary<string, IScriptLibrary>();

            InternalEngine = new ScriptEngine();
            InternalEngine.EnableExposedClrTypes = true;

            InternalEngine.SetGlobalFunction("lib", new Func<string, ObjectInstance>(ImportLibrary));
        }

        private ObjectInstance ImportLibrary(string libraryName)
        {
            if (!_libraryDictionary.ContainsKey(libraryName))
                return null;

            var library = _libraryDictionary[libraryName];

            if (library is ObjectInstance)
                return library as ObjectInstance;

            return new ClrInstanceWrapper(InternalEngine, library);
        }

        public override void SetOutput(IScriptOutput output)
        {
            var console = new FirebugConsole(InternalEngine);
            console.Output = new ScriptOutputProxy(output);
            InternalEngine.SetGlobalValue("console", console);
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
                    InternalEngine.SetGlobalValue("args", InternalEngine.EnumerableToArray(args));

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


        public override ExecutionResult<TReturn> Evaluate<TReturn>(IScriptSource scriptSource, IEnumerable<string> args = null)
        {
            try
            {
                if (args != null)
                    InternalEngine.SetGlobalValue("args", InternalEngine.EnumerableToArray(args));

                TReturn val;
                if (typeof(TReturn).IsArray)
                    val = EvaluateArray<TReturn>(scriptSource);
                else if (typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)) && typeof(TReturn) != typeof(string))
                    val = EvaluateEnumerable<TReturn>(scriptSource);
                else
                    val = InternalEngine.Evaluate<TReturn>(scriptSource.GetReader().ReadToEnd());

                return new ExecutionResult<TReturn>
                {
                    Value = val,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<TReturn>
                {
                    Success = false,
                    ExecutionException = ex
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
            var obj = InternalEngine.Evaluate<ArrayInstance>(scriptSource.GetReader().ReadToEnd()).ElementValues.ToArray();

            var targetType = returnType.GetElementType();
            var cast = castMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { obj });
            return (TReturn)toArrayMethod.MakeGenericMethod(targetType).Invoke(null, new[] { cast });
        }

        private TReturn EvaluateEnumerable<TReturn>(IScriptSource scriptSource)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(typeof(TReturn)))
                return default(TReturn);

            var castMethod = typeof(Enumerable).GetMethod("Cast");
            var toListMethod = typeof(Enumerable).GetMethod("ToList");

            var returnType = typeof(TReturn);
            var obj = InternalEngine.Evaluate<ArrayInstance>(scriptSource.GetReader().ReadToEnd()).ElementValues.ToArray();

            var targetType = returnType.GetGenericArguments()[0];
            var cast = castMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { obj });
            return (TReturn)toListMethod.MakeGenericMethod(targetType).Invoke(null, new[] { cast });
        }


        protected override void Dispose(bool disposing)
        {
            InternalEngine = null;
        }
    }
}