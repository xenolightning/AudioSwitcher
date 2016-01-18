using System;
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
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddCoreLibrary();
                engine.Execute("Core = require('Core');");
                Assert.NotNull(engine.Resolve("Core"));
            }
        }

        [Fact]
        public void Core_sleep_Exists()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddCoreLibrary();
                engine.Execute("Core = require('Core');");
                engine.Execute("Core.sleep(100)");
            }
        }

        [Fact]
        public void Core_Create_Guid()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddCoreLibrary();
                engine.Execute("Core = require('Core');");
                Assert.NotNull(engine.Evaluate<Guid>("Core.createGuid()"));
            }
        }

    }
}
