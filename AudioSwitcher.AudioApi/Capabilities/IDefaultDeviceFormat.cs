using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface IDefaultDeviceFormat : IDeviceCapability
    {

        SampleRate SampleRate
        {
            get;
        }

        BitDepth BitDepth
        {
            get;
        }

        bool DeviceFormatIsValid(SampleRate sampleRate, BitDepth bitDepth);

        void SetDeviceFormat(SampleRate sampleRate, BitDepth bitDepth);

    }
}
