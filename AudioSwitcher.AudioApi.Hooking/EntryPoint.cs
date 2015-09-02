using System;
using System.Runtime.InteropServices;
using System.Threading;
using AudioSwitcher.AudioApi.Hooking.ComObjects;
using EasyHook;

namespace AudioSwitcher.AudioApi.Hooking
{
    public class EntryPoint : IEntryPoint
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.U4)]
        public delegate int DGetDefaultAudioEndpoint(IMMDeviceEnumerator self, DataFlow dataFlow, Role role, out IntPtr ppEndpoint);

        public readonly RemoteInterface Interface;

        public EntryPoint(RemoteHooking.IContext inContext, string inChannelName)
        {
            Interface = RemoteHooking.IpcConnectClient<RemoteInterface>(inChannelName);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                //Create the DefaultDevice Hook
                var cci = new ComClassQuery.ComClassInfo(typeof(MMDeviceEnumeratorComObject), typeof(IMMDeviceEnumerator), "GetDefaultAudioEndpoint");
                ComClassQuery.Query(cci);

                var hook = LocalHook.Create(cci.FunctionPointer, new DGetDefaultAudioEndpoint(GetDefaultAudioEndpoint),
                    this);
                hook.ThreadACL.SetExclusiveACL(new[] { 1 });

            }
            catch (Exception e)
            {
                ReportError(Interface, e);
            }

            //Signal the hook installed, and get the response from the server
            if (!Interface.HookInstalled())
                return;

            try
            {
                while (!Interface.CanUnload())
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                ReportError(Interface, e);
            }

            Interface.HookUninstalled();
        }

        private static int GetDefaultAudioEndpoint(IMMDeviceEnumerator self, DataFlow dataflow, Role role,
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