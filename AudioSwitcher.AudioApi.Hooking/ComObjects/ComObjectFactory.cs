using System;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    internal static class ComObjectFactory
    {

        public static IMultimediaDeviceEnumerator GetDeviceEnumerator()
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComIIds.DEVICE_ENUMERATOR_CID))) as IMultimediaDeviceEnumerator;
        }


        public static IPolicyConfigUnknown GetPolicyConfig()
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComIIds.POLICY_CONFIG_CID))) as IPolicyConfigUnknown;
        }

    }
}
