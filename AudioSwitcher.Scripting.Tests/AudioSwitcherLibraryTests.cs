using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class AudioSwitcherLibraryTests
    {

        public static AudioController GetAudioController()
        {
            return new TestAudioController(new TestDeviceEnumerator(2, 2));
        }

        [Fact]
        public void Engine_AddLibrary_AudioSwitcher()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.Equal(true, engine.InternalEngine.HasGlobalValue("AudioSwitcher"));
            }
        }

        /*
                [Fact]
                public void Engine_Add_Lazy_Function()
                {
                    var engine = new ScriptEngine();
                    const string js = "AudioSwitcher.Lazy();";
                    const string result = "RESULT";

                    var asLib = engine.AddAudioSwitcherLibrary(GetAudioController());
                    asLib.AddFunction("Lazy", () => result);


                    Assert.DoesNotThrow(() => engine.Execute(js));
                    var output = engine.Evaluate<string>(js);
                    Assert.Equal(result, output);
                }

                [Fact]
                public void Engine_Add_Lazy_Function_To_Global()
                {
                    var engine = new ScriptEngine();
                    const string js = "Lazy();";
                    const string result = "RESULT";

                    engine.Global.AddFunction("Lazy", () => result);

                    Assert.DoesNotThrow(() => engine.Execute(js));
                    var output = engine.Evaluate<string>(js);
                    Assert.Equal(result, output);
                }
        */
        //[Fact]
        //public void Engine_RemoveLibrary_AudioSwitcher()
        //{
        //    var engine = new ScriptEngine();
        //    var asLib = engine.AddAudioSwitcherLibrary(GetAudioController());
        //    engine.RemoveLibrary(asLib);

        //    Assert.Equal(engine.GetGlobalValue(asLib.Name), Undefined.Value);
        //}

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices()"));
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_All()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices(0)"));
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices(1)"));
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices(2)"));
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName()
        {
            const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices()[0].name);";

            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute(js));

                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices()[0]");
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js);

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.ID, resolvedAudioDevice.ID);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName_Playback_Flags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(1)[0].name, 1);";

                Assert.DoesNotThrow(() => engine.Execute(js));
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(1)[0]");
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js);

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.ID, resolvedAudioDevice.ID);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName_Capture_Flags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(2)[0].name, 2);";

                Assert.DoesNotThrow(() => engine.Execute(js));
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(2)[0]");
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js);

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.ID, resolvedAudioDevice.ID);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        public void AudioSwitcher_AudioDevice_Exists()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0];";

                Assert.DoesNotThrow(() => engine.Execute(js));
                Assert.NotEqual(null, engine.Evaluate<JavaScriptAudioDevice>(js));
                Assert.IsType<JavaScriptAudioDevice>(engine.Evaluate<JavaScriptAudioDevice>(js));
            }
        }

        [Fact]
        [Trait("Function", "Mute")]
        public void AudioSwitcher_AudioDevice_toggleMute()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].toggleMute()";

                //Toggles the mute and tests non equality of state
                var isMuted = engine.Evaluate<bool>(js);
                Assert.NotEqual(isMuted, engine.Evaluate<bool>(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "Mute")]
        public void AudioSwitcher_AudioDevice_setMute_true()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].mute(true)";

                //Sets to muted
                Assert.Equal(true, engine.Evaluate<bool>(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "Mute")]
        public void AudioSwitcher_AudioDevice_setMute_false()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].mute(false)";
                Assert.Equal(false, engine.Evaluate<bool>(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "Volume")]
        public void AudioSwitcher_AudioDevice_getVolume()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].volume()";

                //don't care what it returns, just that it exists
                Assert.DoesNotThrow(() => engine.Execute(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "Volume")]
        public void AudioSwitcher_AudioDevice_setVolume()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setTo10 = @"AudioSwitcher.getAudioDevices()[0].volume(10)";
                const string getVolume = @"AudioSwitcher.getAudioDevices()[0].volume()";
                var orignalVol = engine.Evaluate<int>(getVolume);
                string setToOriginal = @"AudioSwitcher.getAudioDevices()[0].volume(" + orignalVol + ")";

                Assert.Equal(10, engine.Evaluate<int>(setTo10));
                Assert.Equal(10, engine.Evaluate<int>(getVolume));
                Assert.Equal(orignalVol, engine.Evaluate<int>(setToOriginal));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "Name")]
        public void AudioSwitcher_AudioDevice_getName()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].name";
                Assert.DoesNotThrow(() => engine.Execute(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "ID")]
        public void AudioSwitcher_AudioDevice_getID()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].id";
                Assert.DoesNotThrow(() => engine.Execute(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "ID")]
        public void AudioSwitcher_AudioDevice_getFlags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].flags";
                Assert.DoesNotThrow(() => engine.Execute(js));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefault")]
        public void AudioSwitcher_AudioDevice_setDefault_Playback()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(1)[0].setAsDefaultDevice()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(1)[0].isDefault";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefaultComm")]
        public void AudioSwitcher_AudioDevice_setDefault_Playback_Comm()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(1)[0].setAsDefaultCommDevice()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(1)[0].isDefaultComm";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefault")]
        public void AudioSwitcher_AudioDevice_setDefault_Capture()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(2)[0].setAsDefaultDevice()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(2)[0].isDefault";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault));
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefaultComm")]
        public void AudioSwitcher_AudioDevice_setDefault_Capture_Comm()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(2)[0].setAsDefaultCommDevice()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(2)[0].isDefaultComm";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault));
            }
        }
    }
}
