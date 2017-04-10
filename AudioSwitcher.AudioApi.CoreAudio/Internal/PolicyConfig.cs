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

                var policyConfig7 = policyConfig as IPolicyConfig;
                var policyConfigVista = policyConfig as IPolicyConfigVista;
                var policyConfigX = policyConfig as IPolicyConfigX;
                var policyConfigRedstone = policyConfig as IPolicyConfigRedstone;
                var policyConfigRedstone2 = policyConfig as IPolicyConfigRedstone2;

                if (policyConfigRedstone2 != null)
                {
                    policyConfigRedstone2.SetDefaultEndpoint(devId, eRole);
                }
                else if (policyConfigRedstone != null)
                {
                    policyConfigRedstone.SetDefaultEndpoint(devId, eRole);
                }
                else if (policyConfigX != null)
                {
                    policyConfigX.SetDefaultEndpoint(devId, eRole);
                }
                else if (policyConfig7 != null)
                {
                    policyConfig7.SetDefaultEndpoint(devId, eRole);
                }
                else if (policyConfigVista != null)
                {
                    policyConfigVista.SetDefaultEndpoint(devId, eRole);
                }
                else
                {
                    var policyConfigUnknown = policyConfig as IPolicyConfigUnknown;
                    policyConfigUnknown?.SetDefaultEndpoint(devId, eRole);
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