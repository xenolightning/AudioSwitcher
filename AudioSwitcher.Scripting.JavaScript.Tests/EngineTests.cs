using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class EngineTests
    {
        [Fact]
        public void Engine_Create()
        {
            var engine = new JsEngine();
            Assert.NotNull(engine);
        }

        [Fact]
        public void Engine_WithArgs()
        {

            var test = new[]
            {
                "One",
                "Two"
            };

            var engine = new JsEngine();
            using (var context = engine.CreateExecutionContext(test))
            {
                Assert.NotNull(context.Evaluate<string[]>("args", test));
            }
        }
    }
}
