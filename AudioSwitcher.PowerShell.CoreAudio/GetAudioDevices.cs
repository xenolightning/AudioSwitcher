using System.Collections.Generic;
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


        protected override void BeginProcessing()
        {
            _controller = new CoreAudioController();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<IDevice> devices;

            switch (Type)
            {
                default:
                    devices = _controller.GetDevices();
                    break;
                case DeviceType.Playback:
                    devices = _controller.GetPlaybackDevices();
                    break;
                case DeviceType.Capture:
                    devices = _controller.GetCaptureDevices();
                    break;
            }

            foreach (var device in devices)
                WriteObject(device);
        }

        protected override void EndProcessing()
        {
            if (_controller != null)
                _controller.Dispose();
        }

    }
}
