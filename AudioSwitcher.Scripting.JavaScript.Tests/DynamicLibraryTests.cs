using AudioSwitcher.AudioApi;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class DynamicLibraryTests
    {

        public static IAudioController GetAudioController()
        {
            return new TestDeviceController(2, 2);
        }

        [Fact]
        public void Engine_AddLibrary_Dynamic()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddLibrary("Dynamic", new TestLibrary());

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));
            }
        }

        [Fact]
        public void Engine_Dynamic_Value_Exists()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));
                Assert.Equal(testLib.Property, engine.Evaluate<int>("Dyanmic.Property").Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Delegate_Returns()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("import('Dyanmic');");

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<int>("Dynamic.Delegate()");

                Assert.Equal(testLib.Property, result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Delegate_Func_Fail()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.DelegateWithArguments('Hello')");

                //Assert.NotNull(result.ExecutionException);
                Assert.Equal(testLib.DelegateWithArguments("Hello"), result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Returns()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.Method()");
                
                Assert.Equal(testLib.Method(), result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Returns_Clr()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<int>("Dynamic.MethodReturnClr().Field");

                Assert.Equal(new ClrObject().Field, result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Returns_ClrArray()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<int>("Dynamic.MethodReturnClrArray().length");

                Assert.Equal(2, result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_WithArg_Returns()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.MethodWithArg('Hello')");

                Assert.Equal("Hello", result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_WithArgs_Returns()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.MethodWithArgs('One', 'Two')");

                Assert.Equal("One + Two", result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Func_Returns()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.NotNull(engine.Resolve("Dynamic"));

                var result = engine.Evaluate<int>("(Dynamic.MethodFunc())()");

                //Assert.NotNull(result.ExecutionException);
                Assert.Equal(testLib.MethodFunc()(), result.Value);
            }
        }


    }
}
