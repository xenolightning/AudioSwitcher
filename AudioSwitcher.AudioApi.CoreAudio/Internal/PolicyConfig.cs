using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

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

                switch (policyConfig)
                {
                    case IPolicyConfigRedstone3 config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
                    case IPolicyConfigRedstone2 config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
                    case IPolicyConfigRedstone config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
                    case IPolicyConfigX config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
                    case IPolicyConfig config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
                    case IPolicyConfigVista config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
                    case IPolicyConfigUnknown config:
                        config.SetDefaultEndpoint(devId, eRole);
                        break;
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