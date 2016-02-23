using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface ISpeakerConfiguration : IDeviceCapability
    {

        SpeakerConfiguration SpeakerConfiguration
        {
            get;
        }

        void IsSupported(SpeakerConfiguration configuration);

        void SetSpeakerConfiguration(SpeakerConfiguration configuration);

    }
}
