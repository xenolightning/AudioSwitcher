using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

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

                // ReSharper disable once SuspiciousTypeConversion.Global
                var policyConfigX = policyConfig as IPolicyConfigX;
                if (policyConfigX != null)
                    Marshal.ThrowExceptionForHR(policyConfigX.SetDefaultEndpoint(devId, eRole));

                // Try the Windows 7 Api Reference
                // ReSharper disable once SuspiciousTypeConversion.Global
                var policyConfig7 = policyConfig as IPolicyConfig;
                if (policyConfig7 != null)
                    Marshal.ThrowExceptionForHR(policyConfig7.SetDefaultEndpoint(devId, eRole));

                //Try the Vista Api Reference

                // ReSharper disable once SuspiciousTypeConversion.Global
                var policyConfigVista = policyConfig as IPolicyConfigVista;
                if (policyConfigVista != null)
                    Marshal.ThrowExceptionForHR(policyConfigVista.SetDefaultEndpoint(devId, eRole));
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