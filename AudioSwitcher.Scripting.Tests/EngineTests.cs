using Jurassic;
using Xunit;

namespace AudioSwitcher.Scripting.Tests
{
    public class EngineTests
    {
        [Fact]
        public void Engine_Create()
        {
            var engine = new ScriptEngine();
            Assert.NotNull(engine);
        }

    }
}
