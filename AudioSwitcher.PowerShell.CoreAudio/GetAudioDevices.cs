using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcher.PowerShell.CoreAudio
{

    [Cmdlet(VerbsCommon.Get, "AudioDevices")]
    public class GetAudioDevices : Cmdlet
    {
        private IAudioController _controller;

        [Parameter]
        public DeviceType Type
        {
            get;
            set;
        }

        public GetAudioDevices()
        {
            _controller = new CoreAudioController();
        }

        protected override void ProcessRecord()
        {
            IDevice[] devices;

            switch (Type)
            {
                default:
                    devices = _controller.GetDevices().ToArray();
                    break;
                case DeviceType.Playback:
                    devices = _controller.GetPlaybackDevices().ToArray();
                    break;
                case DeviceType.Capture:
                    devices = _controller.GetCaptureDevices().ToArray();
                    break;
            }
            
            WriteObject(devices);
        }
    }
}
