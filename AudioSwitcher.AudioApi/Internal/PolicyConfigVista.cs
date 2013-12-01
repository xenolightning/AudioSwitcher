using System.Runtime.InteropServices;
// This has been modified for use by Audio Switcher
using AudioSwitcher.AudioApi.Interfaces;

namespace AudioSwitcher.AudioApi
{
    internal static class PolicyConfigVista
    {
        //private readonly IPolicyConfigVista _IPolicyConfigVista = new CPolicyConfigVistaClient() as IPolicyConfigVista;

        public static void SetDefaultEndpoint(string wszDeviceId, Role eRole)
        {
            var _IPolicyConfigVista = new CPolicyConfigVistaClient() as IPolicyConfigVista;
            Marshal.ThrowExceptionForHR(_IPolicyConfigVista.SetDefaultEndpoint(wszDeviceId, eRole));
        }

        [ComImport, Guid("294935CE-F637-4E7C-A41B-AB255460B862")]
        private class CPolicyConfigVistaClient
        {
        }
    }
}