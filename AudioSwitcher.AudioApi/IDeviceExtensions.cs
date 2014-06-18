using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioSwitcher.AudioApi.Plugins;

namespace AudioSwitcher.AudioApi
{
    public static class IDeviceExtensions
    {

        public static bool IsPreferredDevice(this IDevice device)
        {
            var pdm = device.Enumerator.AudioController.GetPlugin<IPreferredDeviceManager>();
            if (pdm == null)
                return false;

            return pdm.IsPreferredDevice(device);
        }


        public static bool SetPreferredDevice(this IDevice device)
        {
            var pdm = device.Enumerator.AudioController.GetPlugin<IPreferredDeviceManager>();
            if (pdm == null)
                return false;

            pdm.AddDevice(device);
            return true;
        }

        public static bool RemoveAsPreferredDevice(this IDevice device)
        {
            var pdm = device.Enumerator.AudioController.GetPlugin<IPreferredDeviceManager>();
            if (pdm == null)
                return false;

            pdm.RemoveDevice(device);
            return true;
        }
    }
}
