using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using AudioSwitcher.Tests.Common;
using Jurassic.Library;
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


                    ExecutionResult result = null;
Assert.DoesNotThrow(() => result = engine.Execute(js));
Assert.NotNull(result);
Assert.Null(result.ExecutionException);
Assert.True(result.Success);
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

                    ExecutionResult result = null;
Assert.DoesNotThrow(() => result = engine.Execute(js));
Assert.NotNull(result);
Assert.Null(result.ExecutionException);
Assert.True(result.Success);
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
                Assert.Equal(4, engine.Evaluate<int>("AudioSwitcher.getAudioDevices().length").Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_All()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.ALL)"));
                Assert.Equal(4, engine.Evaluate<int>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.ALL).length").Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)"));
                Assert.Equal(2, engine.Evaluate<int>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK).length").Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback_IsPlayback()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Result;
                var devices = engine.Evaluate<JavaScriptAudioDevice[]>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)").Result;
                Assert.True(devices.All(x => x.IsPlayback));
                Assert.True(device.IsPlayback);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback_IsPlayback_Enum()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Result;
                var devices = engine.Evaluate<IEnumerable<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)").Result;
                Assert.True(devices.All(x => x.IsPlayback));
                Assert.True(device.IsPlayback);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                Assert.DoesNotThrow(() => engine.Execute("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)"));
                Assert.Equal(2, engine.Evaluate<int>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE).length").Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var devices = engine.Evaluate<JavaScriptAudioDevice[]>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Result;
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_Enum()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var devices = engine.Evaluate<IEnumerable<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Result;
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_IList()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var devices = engine.Evaluate<IList<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Result;
                Assert.IsAssignableFrom<IList<JavaScriptAudioDevice>>(devices);
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_List()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var devices = engine.Evaluate<List<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Result;
                Assert.IsAssignableFrom<IEnumerable<JavaScriptAudioDevice>>(devices);
                Assert.IsAssignableFrom<IList<JavaScriptAudioDevice>>(devices);
                Assert.IsType<List<JavaScriptAudioDevice>>(devices);
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_ICollection()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var devices = engine.Evaluate<ICollection<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Result;
                Assert.IsAssignableFrom<ICollection<JavaScriptAudioDevice>>(devices);
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceById()
        {
            const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices()[0].id);";

            using (var engine = new JSEngine(GetAudioController()))
            {
                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);

                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices()[0]").Result;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Result;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceById_Playback_Flags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].id, AudioSwitcher.DeviceType.PLAYBACK);";

                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Result;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Result;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsPlayback);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceById_Capture_Flags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].id, AudioSwitcher.DeviceType.CAPTURE);";

                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Result;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName()
        {
            const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices()[0].name);";

            using (var engine = new JSEngine(GetAudioController()))
            {
                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);

                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices()[0]").Result;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Result;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName_Playback_Flags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].name, AudioSwitcher.DeviceType.PLAYBACK);";

                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Result;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Result;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsPlayback);
            }
        }

        [Fact]
        [Trait("Type", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName_Capture_Flags()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].name, AudioSwitcher.DeviceType.CAPTURE);";

                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Result;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Result;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsCapture);
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        public void AudioSwitcher_AudioDevice_Exists()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0];";

                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                Assert.NotEqual(null, engine.Evaluate<JavaScriptAudioDevice>(js));
                Assert.IsType<JavaScriptAudioDevice>(engine.Evaluate<JavaScriptAudioDevice>(js).Result);
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
                var isMuted = engine.Evaluate<bool>(js).Result;
                Assert.NotEqual(isMuted, engine.Evaluate<bool>(js).Result);
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
                Assert.Equal(true, engine.Evaluate<bool>(js).Result);
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
                Assert.Equal(false, engine.Evaluate<bool>(js).Result);
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
                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
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
                var orignalVol = engine.Evaluate<int>(getVolume).Result;
                string setToOriginal = @"AudioSwitcher.getAudioDevices()[0].volume(" + orignalVol + ")";

                Assert.Equal(10, engine.Evaluate<int>(setTo10).Result);
                Assert.Equal(10, engine.Evaluate<int>(getVolume).Result);
                Assert.Equal(orignalVol, engine.Evaluate<int>(setToOriginal).Result);
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
                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
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
                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
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
                ExecutionResult result = null;
                Assert.DoesNotThrow(() => result = engine.Execute(js));
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefault")]
        public void AudioSwitcher_AudioDevice_setDefault_Playback()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].setAsDefault()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].isDefault";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefaultComm")]
        public void AudioSwitcher_AudioDevice_setDefault_Playback_Comm()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].setAsDefaultComm()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].isDefaultComm";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefault")]
        public void AudioSwitcher_AudioDevice_setDefault_Capture()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].setAsDefault()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].isDefault";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Result);
            }
        }

        [Fact]
        [Trait("Type", "AudioDevice")]
        [Trait("Function", "SetDefaultComm")]
        public void AudioSwitcher_AudioDevice_setDefault_Capture_Comm()
        {
            using (var engine = new JSEngine(GetAudioController()))
            {
                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].setAsDefaultComm()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].isDefaultComm";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Result);
            }
        }
    }
}
