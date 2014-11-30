// This has been modified for use by Audio Switcher

using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PolicyConfig
    {
        public static void SetDefaultEndpoint(string devId, ERole eRole)
        {
            var policyConfig = new _PolicyConfigClient() as IPolicyConfig;
            if (policyConfig != null)
                Marshal.ThrowExceptionForHR(policyConfig.SetDefaultEndpoint(devId, eRole));
        }

        [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        private class _PolicyConfigClient
        {
        }
    }
}