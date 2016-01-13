using Jint.Native;
using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class ConsoleTests
    {

        [Fact]
        public void Engine_Console_Exists()
        {
            using (var engine = new JsEngine())
            {
                Assert.NotEqual(JsValue.Undefined, engine.InternalEngine.GetValue("console"));
            }
        }
        [Fact]
        public void Engine_Console_Logs_Strings()
        {
            using (var engine = new JsEngine())
            {
                engine.SetOutput(new ConsoleScriptOutput());
                var result = engine.Execute("console.log('hi', 'hi');");

                Assert.Null(result.ExecutionException);
            }
        }

        [Fact]
        public void Engine_Console_Logs_Objects()
        {
            using (var engine = new JsEngine())
            {
                engine.SetOutput(new ConsoleScriptOutput());
                var result = engine.Execute("console.log('hi', { hi: 'hi' });");

                Assert.Null(result.ExecutionException);
            }
        }

    }
}
