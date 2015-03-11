using System;
using System.Linq;
using System.Management.Automation;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcher.PowerShell.CoreAudio
{

    [Cmdlet(VerbsCommon.Get, "AudioController")]
    public class GetAudioController : Cmdlet, IDisposable
    {
        private readonly IAudioController _controller;

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

        public GetAudioController()
        {
            _controller = new CoreAudioController();
        }

        protected override void ProcessRecord()
        {
            WriteObject(_controller);
        }

        public void Dispose()
        {
            if(_controller != null)
                _controller.Dispose();
        }
    }
}
