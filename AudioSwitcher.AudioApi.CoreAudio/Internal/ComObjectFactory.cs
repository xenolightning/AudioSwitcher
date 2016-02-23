using System;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class ComObjectFactory
    {
        public static IMultimediaDeviceEnumerator GetDeviceEnumerator()
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComInterfaceIds.DEVICE_ENUMERATOR_CID))) as IMultimediaDeviceEnumerator;
        }

        public static object GetPolicyConfig()
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComInterfaceIds.POLICY_CONFIG_CID)));
        }

    }
}
