using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

// This has been modified for use by Audio Switcher

namespace NAudio.CoreAudioApi
{
    internal static class PolicyConfig
    {
        //private readonly IPolicyConfig _PolicyConfig = new _PolicyConfigClient() as IPolicyConfig;

        public static void SetDefaultEndpoint(string devID, Role eRole)
        {
            var _PolicyConfig = new _PolicyConfigClient() as IPolicyConfig;
            Marshal.ThrowExceptionForHR(_PolicyConfig.SetDefaultEndpoint(devID, eRole));
        }

        [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        private class _PolicyConfigClient
        {
        }
    }
}