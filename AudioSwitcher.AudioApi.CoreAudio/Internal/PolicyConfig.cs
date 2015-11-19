using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

// ReSharper disable SuspiciousTypeConversion.Global

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PolicyConfig
    {
        public static void SetDefaultEndpoint(string devId, ERole eRole)
        {
            _PolicyConfigClient policyConfig = null;
            try
            {
                policyConfig = new _PolicyConfigClient();

                var policyConfigX = policyConfig as IPolicyConfigX;
                var policyConfig7 = policyConfig as IPolicyConfig;
                var policyConfigVista = policyConfig as IPolicyConfigVista;

                if (policyConfigX != null)
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
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (policyConfig != null && Marshal.IsComObject(policyConfig))
                    Marshal.FinalReleaseComObject(policyConfig);

                GC.Collect();
            }
        }

        [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        private class _PolicyConfigClient
        {
        }
    }
}