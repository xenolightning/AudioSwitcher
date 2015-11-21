using System;
using System.Runtime.InteropServices;
using System.Threading;
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
                    //test
                    //WaveFormatExtensible aasdasd;
                    //policyConfigX.GetDeviceFormat(devId, true, out aasdasd);
                    //WaveFormatExtensible mix;
                    //policyConfigX.GetMixFormat(devId, out mix);

                    //aasdasd = new WaveFormatExtensible(44100, 16, (int)SpeakerConfiguration.Stereo);
                    //mix = new WaveFormatExtensible(44100, 16, (int)SpeakerConfiguration.Stereo);

                    ////aasdasd.SampleRate = 96000;
                    ////mix.SampleRate = 96000;

                    ////i think we need to set the property value here too

                    //policyConfigX.SetDeviceFormat(devId, aasdasd, mix);
                    //Thread.Sleep(1000);
                    //policyConfigX.SetShareMode(devId, DeviceShareMode.Shared);
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