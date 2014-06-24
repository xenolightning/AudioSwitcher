using Jurassic;
using Xunit;

namespace AudioSwitcher.Scripting.Tests
{
    public class CoreLibraryTests
    {

        [Fact]
        public void Engine_AddLibrary_Core()
        {
            var engine = new ScriptEngine();
            var coreLib = engine.AddCoreLibrary();

            Assert.Equal(true, engine.HasGlobalValue(coreLib.Name));
        }

        [Fact]
        public void Engine_RemoveLibrary_Core()
        {
            var engine = new ScriptEngine();
            var coreLib = engine.AddCoreLibrary();
            engine.RemoveLibrary(coreLib);

            Assert.Equal(engine.GetGlobalValue(coreLib.Name), Undefined.Value);
        }

        [Fact]
        public void Core_sleep_Exists()
        {
            var engine = new ScriptEngine();
            engine.AddCoreLibrary();
            Assert.DoesNotThrow(() => engine.Execute("Core.sleep(10)"));
        }

    }
}
