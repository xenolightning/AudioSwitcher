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

    [Cmdlet(VerbsCommon.Get, "AudioDevice")]
    public class GetAudioDevice : Cmdlet
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

        public GetAudioDevice()
        {
            _controller = new CoreAudioController();
        }

        protected override void ProcessRecord()
        {
            if (Id.HasValue)
                WriteObject(_controller.GetDevice(Id.Value));
            else
                WriteObject(_controller.GetDevices().FirstOrDefault(x => String.Equals(x.Name, Name, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
