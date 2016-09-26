using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

// ReSharper disable SuspiciousTypeConversion.Global

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PolicyConfig
    {
        public static void SetDefaultEndpoint(string devId, ERole eRole)
        {
            object policyConfig = null;
            try
            {
                policyConfig = ComObjectFactory.GetPolicyConfig();

                var policyConfigX = policyConfig as IPolicyConfigX;
                var policyConfig7 = policyConfig as IPolicyConfig;
                var policyConfigVista = policyConfig as IPolicyConfigVista;

                if (policyConfig7 != null)
                {
                    policyConfig7.SetDefaultEndpoint(devId, eRole);
                }
                else if (policyConfigVista != null)
                {
                    policyConfigVista.SetDefaultEndpoint(devId, eRole);
                }
                else
                {
                    policyConfigX?.SetDefaultEndpoint(devId, eRole);
                }
            }
            finally
            {
                if (policyConfig != null && Marshal.IsComObject(policyConfig))
                    Marshal.FinalReleaseComObject(policyConfig);
            }
        }
    }
}