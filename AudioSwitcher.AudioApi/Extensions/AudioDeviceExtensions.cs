using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioSwitcher.AudioApi.Extensions
{
    public static class AudioDeviceExtensions
    {

        public static bool IsPreferredDevice(this IAudioDevice ad)
        {
            var pdm = ad.Enumerator.Controller.Context.PreferredDeviceManager;
            return pdm != null && pdm.IsPreferredDevice(ad);
        }

    }
}
