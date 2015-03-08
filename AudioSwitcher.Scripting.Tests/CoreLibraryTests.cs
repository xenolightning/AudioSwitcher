using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class CoreLibraryTests
    {
        public static IAudioController GetAudioController()
        {
            return new TestDeviceController(2, 2);
        }

        [Fact]
        public void Engine_AddLibrary_Core()
        {
            using (var engine = new JSEngine())
            {
                engine.AddCoreLibrary();
                engine.Execute("Core = lib('AudioSwitcher');");
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Core"));
            }
        }

        [Fact]
        public void Core_sleep_Exists()
        {
            using (var engine = new JSEngine())
            {
                engine.AddCoreLibrary();
                engine.Execute("Core = lib('Core');");
                Assert.DoesNotThrow(() => engine.Execute("Core.sleep(100)"));
            }
        }

    }
}
