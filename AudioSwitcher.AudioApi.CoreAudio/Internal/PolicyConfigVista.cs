// This has been modified for use by Audio Switcher

using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PolicyConfigVista
    {

        public static void SetDefaultEndpoint(string devId, ERole eRole)
        {
            IPolicyConfig policyConfig = null;
            try
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                policyConfig = new _PolicyConfigVistaClient() as IPolicyConfig;
                if (policyConfig != null)
                    Marshal.ThrowExceptionForHR(policyConfig.SetDefaultEndpoint(devId, eRole));
            }
            finally
            {
                if (policyConfig != null && Marshal.IsComObject(policyConfig))
                    Marshal.ReleaseComObject(policyConfig);
            }
        }

        [ComImport, Guid("294935CE-F637-4E7C-A41B-AB255460B862")]
        private class _PolicyConfigVistaClient
        {
        }
    }
}