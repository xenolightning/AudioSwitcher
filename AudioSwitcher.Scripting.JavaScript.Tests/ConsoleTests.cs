using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class ConsoleTests
    {

        [Fact]
        public void Engine_Console_Exists()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                Assert.NotNull(engine.Resolve("console"));
            }
        }
        [Fact]
        public void Engine_Console_Logs_Strings()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.SetOutput(new ConsoleScriptOutput());
                var result = engine.Execute("console.log('hi', 'hi');");

                Assert.Null(result.ExecutionException);
            }
        }

        [Fact]
        public void Engine_Console_Logs_Objects()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.SetOutput(new ConsoleScriptOutput());
                var result = engine.Execute("console.log('hi', { hi: 'hi' });");

                Assert.Null(result.ExecutionException);
            }
        }

    }
}
