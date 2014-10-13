using AudioSwitcher.AudioApi;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class CoreLibraryTests
    {
        public static AudioController GetAudioController()
        {
            return new TestAudioController(new TestDeviceEnumerator(2, 2));
        }

        [Fact]
        public void Engine_AddLibrary_Core()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("Core"));
            }
        }

        [Fact]
        public void Core_sleep_Exists()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("Core.sleep(100)"));
            }
        }
        
    }
}
