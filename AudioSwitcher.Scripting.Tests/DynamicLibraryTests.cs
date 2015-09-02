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
            using (var engine = new JsEngine())
            {
                engine.AddLibrary("Dynamic", new TestLibrary());

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));
            }
        }

        [Fact]
        public void Engine_Dynamic_Value_Exists()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));
                Assert.Equal(testLib.Property, engine.Evaluate<int>("Dyanmic.Property").Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Delegate_Returns()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("import('Dyanmic');");

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<int>("Dynamic.Delegate()");

                Assert.Equal(testLib.Property, result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Delegate_Func_Fail()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.True(engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.DelegateWithArguments('Hello')");

                Assert.NotNull(result.ExecutionException);
                //Assert.Equal(testLib.DelegateWithArguments(), result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Returns()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.Method()");
                
                Assert.Equal(testLib.Method(), result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Returns_Clr()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<int>("Dynamic.MethodReturnClr().Field");

                Assert.Equal(new ClrObject().Field, result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Returns_ClrArray()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<int>("Dynamic.MethodReturnClrArray().Length");

                Assert.Equal(2, result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_WithArg_Returns()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.MethodWithArg('Hello')");

                Assert.Equal("Hello", result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_WithArgs_Returns()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<string>("Dynamic.MethodWithArgs('One', 'Two')");

                Assert.Equal("One + Two", result.Value);
            }
        }

        [Fact]
        public void Engine_Dynamic_Method_Func_Returns()
        {
            using (var engine = new JsEngine())
            {
                var testLib = new TestLibrary();
                engine.AddLibrary("Dynamic", testLib);

                engine.Execute("Dynamic = require('Dynamic');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Dynamic"));

                var result = engine.Evaluate<string>("(Dynamic.MethodFunc())()");

                Assert.NotNull(result.ExecutionException);
                //Assert.Equal(testLib.MethodFunc()(), result.Value);
            }
        }


    }
}
