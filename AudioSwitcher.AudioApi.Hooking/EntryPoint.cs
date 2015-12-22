using System;
using System.Runtime.InteropServices;
using System.Threading;
using AudioSwitcher.AudioApi.Hooking.ComObjects;
using EasyHook;

namespace AudioSwitcher.AudioApi.Hooking
{
    public sealed class EntryPoint : IEntryPoint
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.U4)]
        public delegate int DGetDefaultAudioEndpoint(IMultimediaDeviceEnumerator self, DataFlow dataFlow, Role role, out IntPtr ppEndpoint);

        public readonly RemoteInterface Interface;

        public EntryPoint(RemoteHooking.IContext inContext, string inChannelName)
        {
            Interface = RemoteHooking.IpcConnectClient<RemoteInterface>(inChannelName);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            LocalHook hook = null;
            try
            {
                //Create the DefaultDevice Hook
                var cci = new COMClassInfo(Type.GetTypeFromCLSID(new Guid(ComIIds.DEVICE_ENUMERATOR_CID)), typeof(IMultimediaDeviceEnumerator), "GetDefaultAudioEndpoint");
                cci.Query();

                hook = LocalHook.Create(cci.MethodPointers[0], new DGetDefaultAudioEndpoint(GetDefaultAudioEndpoint), this);

                hook.ThreadACL.SetExclusiveACL(new[] { 0 });

                //Sleep here so the hook takes effect
                Thread.Sleep(50);

                //Signal the hook installed, and get the response from the server
                if (!Interface.HookInstalled())
                    return;

            }
            catch (Exception e)
            {
                ReportError(Interface, e);
            }

            try
            {
                while (!Interface.CanUnload())
                {
                    Thread.Sleep(200);
                }

                Interface.HookUninstalled(RemoteHooking.GetCurrentProcessId());
            }
            catch (Exception e)
            {
                ReportError(Interface, e);
            }

            if (hook != null)
                hook.Dispose();
        }

        private static int GetDefaultAudioEndpoint(IMultimediaDeviceEnumerator self, DataFlow dataflow, Role role,
            out IntPtr ppendpoint)
        {
            var entryPoint = HookRuntimeInfo.Callback as EntryPoint;

            if (entryPoint == null || entryPoint.Interface == null)
                return self.GetDefaultAudioEndpoint(dataflow, role, out ppendpoint);

            var remoteInterface = entryPoint.Interface;

            try
            {
                var devId = remoteInterface.GetDefaultDevice(dataflow, role);
                return self.GetDevice(devId, out ppendpoint);
            }
            catch (Exception ex)
            {
                ReportError(remoteInterface, ex);
            }

            //Something failed so return the actual default device
            return self.GetDefaultAudioEndpoint(dataflow, role, out ppendpoint);
        }

        private static void ReportError(RemoteInterface remoteInterface, Exception ex)
        {
            try
            {
                remoteInterface.ReportError(RemoteHooking.GetCurrentProcessId(), ex);
            }
            catch
            {
                // ignored
            }
        }
    }
}