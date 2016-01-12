using System;
using System.Management.Automation;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcher.PowerShell.CoreAudio
{

    [Cmdlet(VerbsCommon.Get, "AudioController")]
    public class GetAudioController : Cmdlet
    {
        private IAudioController _controller;

        [Parameter]
        public Guid? Id
        {
            get;
            set;
        }

        [Parameter]
        public string Name
        {
            get;
            set;
        }

        protected override void BeginProcessing()
        {
            //lazy setup of the controll
            _controller = new CoreAudioController();
        }

        protected override void ProcessRecord()
        {
            WriteObject(_controller);
        }

        protected override void EndProcessing()
        {
            if (_controller != null)
                _controller.Dispose();
        }
    }
}
