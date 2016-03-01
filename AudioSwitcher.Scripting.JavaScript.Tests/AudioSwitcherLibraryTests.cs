using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.Scripting.JavaScript.Internal;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class AudioSwitcherLibraryTests
    {

        public static IAudioController GetAudioController()
        {
            return new TestDeviceController(2, 2);
        }

        [Fact]
        public void Engine_AddLibrary_AudioSwitcher()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                Assert.NotNull(engine.Resolve("AudioSwitcher"));
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                engine.Execute("AudioSwitcher.getAudioDevices()");
                Assert.Equal(4, engine.Evaluate<int>("AudioSwitcher.getAudioDevices().length").Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_All()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                engine.Execute("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.ALL)");
                Assert.Equal(4, engine.Evaluate<int>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.ALL).length").Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                engine.Execute("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)");
                Assert.Equal(2, engine.Evaluate<int>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK).length").Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback_IsPlayback()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Value;
                var devices = engine.Evaluate<JavaScriptAudioDevice[]>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)").Value;
                Assert.True(devices.All(x => x.IsPlayback));
                Assert.True(device.IsPlayback);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Playback_IsPlayback_Enum()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Value;
                var devices = engine.Evaluate<IEnumerable<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)").Value;
                Assert.True(devices.All(x => x.IsPlayback));
                Assert.True(device.IsPlayback);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                engine.Execute("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)");
                Assert.Equal(2, engine.Evaluate<int>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE).length").Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var devices = engine.Evaluate<JavaScriptAudioDevice[]>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Value;
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_Enum()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var devices = engine.Evaluate<IEnumerable<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Value;
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_IList()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var devices = engine.Evaluate<IList<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Value;
                Assert.IsAssignableFrom<IList<JavaScriptAudioDevice>>(devices);
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_List()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var devices = engine.Evaluate<List<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Value;
                Assert.IsAssignableFrom<IEnumerable<JavaScriptAudioDevice>>(devices);
                Assert.IsAssignableFrom<IList<JavaScriptAudioDevice>>(devices);
                Assert.IsType<List<JavaScriptAudioDevice>>(devices);
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDevices_Capture_IsCapture_ICollection()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");
                var device = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var devices = engine.Evaluate<ICollection<JavaScriptAudioDevice>>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)").Value;
                Assert.IsAssignableFrom<ICollection<JavaScriptAudioDevice>>(devices);
                Assert.True(devices.All(x => x.IsCapture));
                Assert.True(device.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceById()
        {
            const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices()[0].id);";

            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);

                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices()[0]").Value;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Value;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceById_Playback_Flags()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].id, AudioSwitcher.DeviceType.PLAYBACK);";

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Value;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Value;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsPlayback);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceById_Capture_Flags()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].id, AudioSwitcher.DeviceType.CAPTURE);";

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Value;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName()
        {
            const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices()[0].name);";

            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);

                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices()[0]").Value;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Value;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName_Playback_Flags()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].name, AudioSwitcher.DeviceType.PLAYBACK);";

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0]").Value;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Value;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsPlayback);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioLibrary")]
        public void AudioSwitcher_getAudioDeviceByName_Capture_Flags()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevice(AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].name, AudioSwitcher.DeviceType.CAPTURE);";

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                var audioDevice = engine.Evaluate<JavaScriptAudioDevice>("AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0]").Value;
                var resolvedAudioDevice = engine.Evaluate<JavaScriptAudioDevice>(js).Value;

                Assert.NotEqual(null, resolvedAudioDevice);
                Assert.Equal(audioDevice.Id, resolvedAudioDevice.Id);
                Assert.IsType<JavaScriptAudioDevice>(resolvedAudioDevice);
                Assert.True(resolvedAudioDevice.IsCapture);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        public void AudioSwitcher_AudioDevice_Exists()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0];";

                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
                Assert.NotEqual(null, engine.Evaluate<JavaScriptAudioDevice>(js));
                Assert.IsType<JavaScriptAudioDevice>(engine.Evaluate<JavaScriptAudioDevice>(js).Value);
            }
        }

        [Fact]
        [Trait("Function", "Mute")]
        public void AudioSwitcher_AudioDevice_toggleMute()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0].toggleMute()";

                //Toggles the mute and tests non equality of state
                var isMuted = engine.Evaluate<bool>(js).Value;
                Assert.NotEqual(isMuted, engine.Evaluate<bool>(js).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "Mute")]
        public void AudioSwitcher_AudioDevice_setMute_true()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0].mute(true)";

                //Sets to muted
                Assert.Equal(true, engine.Evaluate<bool>(js).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "Mute")]
        public void AudioSwitcher_AudioDevice_setMute_false()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                const string js = @"AudioSwitcher.getAudioDevices()[0].mute(false)";
                Assert.Equal(false, engine.Evaluate<bool>(js).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "VolumeChanged")]
        public void AudioSwitcher_AudioDevice_getVolume()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0].volume()";

                //don't care what it returns, just that it exists
                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "VolumeChanged")]
        public void AudioSwitcher_AudioDevice_setVolume()
        {
            var eng = new JsEngine();
            using (var context = eng.CreateExecutionContext())
            {
                context.AddAudioSwitcherLibrary(GetAudioController());
                context.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string setTo10 = @"AudioSwitcher.getAudioDevices()[0].volume(10)";
                const string getVolume = @"AudioSwitcher.getAudioDevices()[0].volume()";
                var orignalVol = context.Evaluate<int>(getVolume).Value;
                var setToOriginal = @"AudioSwitcher.getAudioDevices()[0].volume(" + orignalVol + ")";

                Assert.Equal(10, context.Evaluate<int>(setTo10).Value);
                Assert.Equal(10, context.Evaluate<int>(getVolume).Value);
                Assert.Equal(orignalVol, context.Evaluate<int>(setToOriginal).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "Name")]
        public void AudioSwitcher_AudioDevice_getName()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0].name";
                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "ID")]
        public void AudioSwitcher_AudioDevice_getID()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0].id";
                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "ID")]
        public void AudioSwitcher_AudioDevice_getFlags()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string js = @"AudioSwitcher.getAudioDevices()[0].flags";
                var result = engine.Execute(js);
                Assert.NotNull(result);
                Assert.Null(result.ExecutionException);
                Assert.True(result.Success);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "SetDefault")]
        public void AudioSwitcher_AudioDevice_setDefault_Playback()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].setAsDefault()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].isDefault";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "SetDefaultComm")]
        public void AudioSwitcher_AudioDevice_setDefault_Playback_Comm()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].setAsDefaultComm()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.PLAYBACK)[0].isDefaultComm";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "SetDefault")]
        public void AudioSwitcher_AudioDevice_setDefault_Capture()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].setAsDefault()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].isDefault";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Value);
            }
        }

        [Fact]
        [Trait("MediaType", "AudioDevice")]
        [Trait("Function", "SetDefaultComm")]
        public void AudioSwitcher_AudioDevice_setDefault_Capture_Comm()
        {
            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {
                engine.AddAudioSwitcherLibrary(GetAudioController());
                engine.Execute("AudioSwitcher = require('AudioSwitcher');");

                const string setDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].setAsDefaultComm()";
                const string checkDefault = @"AudioSwitcher.getAudioDevices(AudioSwitcher.DeviceType.CAPTURE)[0].isDefaultComm";

                engine.Execute(setDefault);

                Assert.Equal(true, engine.Evaluate<bool>(checkDefault).Value);
            }
        }
    }
}
