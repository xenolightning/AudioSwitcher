using System;
using System.Linq;
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
            {
                WriteObject(_controller.GetDevice(Id.Value));
            }
            else
            {
                var wildCard = new WildcardPattern(Name, WildcardOptions.IgnoreCase);

                WriteObject(
                    _controller.GetDevices()
                        .FirstOrDefault(x => wildCard.IsMatch(x.Name)));
            }
        }
    }
}
