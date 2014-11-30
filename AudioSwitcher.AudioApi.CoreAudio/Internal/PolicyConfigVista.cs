// This has been modified for use by Audio Switcher

using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PolicyConfigVista
    {

        public static void SetDefaultEndpoint(string devId, ERole eRole)
        {
            var iPolicyConfigVista = new _PolicyConfigVistaClient() as IPolicyConfigVista;

            if (iPolicyConfigVista != null)
                Marshal.ThrowExceptionForHR(iPolicyConfigVista.SetDefaultEndpoint(devId, eRole));
        }

        [ComImport, Guid("294935CE-F637-4E7C-A41B-AB255460B862")]
        private class _PolicyConfigVistaClient
        {
        }
    }
}