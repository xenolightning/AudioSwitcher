using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Threading;
using AudioSwitcher.AudioApi.Hooking.ComObjects;
using EasyHook;

namespace AudioSwitcher.AudioApi.Hooking
{
    public class DefaultDeviceHook : IHook, IDisposable
    {
        public delegate void OnErrorHandler(int processId, Exception exception);

        private readonly int _processId;
        private readonly Func<EDataFlow, ERole, string> _systemDeviceId;
        private string _channelName;

        public DefaultDeviceHook(int processId, Func<EDataFlow, ERole, string> systemDeviceId)
        {
            _processId = processId;
            _systemDeviceId = systemDeviceId;
        }

        public bool IsHooked
        {
            get;
            private set;
        }

        public void Dispose()
        {
            UnHook();
        }

        public void Hook()
        {
            if (IsHooked)
                return;

            var ri = new RemoteInterface
            {
                //Wrap the target delegate in our own delegate for reference safety
                SystemId = (x, y) => _systemDeviceId(x, y),
                Unload = () => !IsHooked,
                ErrorHandler = RaiseOnError
            };

            RemoteHooking.IpcCreateServer(ref _channelName, WellKnownObjectMode.Singleton, ri);

            RemoteHooking.Inject(
                _processId,
                InjectionOptions.DoNotRequireStrongName,
                typeof (IMMDeviceEnumerator).Assembly.Location,
                typeof (IMMDeviceEnumerator).Assembly.Location,
                _channelName);

            IsHooked = true;
        }

        public void UnHook()
        {
            if (!IsHooked)
                return;

            IsHooked = false;
        }

        public event OnErrorHandler OnError;

        private void RaiseOnError(int processId, Exception exception)
        {
            if (OnError != null)
                OnError(processId, exception);
        }

        public class EntryPoint : IEntryPoint
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
            [return: MarshalAs(UnmanagedType.U4)]
            public delegate int DGetDefaultAudioEndpoint(
                IMMDeviceEnumerator self, EDataFlow eDataFlow, ERole eRole, out IntPtr ppEndpoint);

            public readonly RemoteInterface Interface;

            public EntryPoint(RemoteHooking.IContext inContext, string inChannelName)
            {
                Interface = RemoteHooking.IpcConnectClient<RemoteInterface>(inChannelName);
            }

            public void Run(RemoteHooking.IContext inContext, string inChannelName)
            {
                //Create the DefaultDevice Hook
                var cci = new ComClassQuery.ComClassInfo(typeof (MMDeviceEnumeratorComObject),
                    typeof (IMMDeviceEnumerator), "GetDefaultAudioEndpoint");
                ComClassQuery.Query(cci);

                var hook = LocalHook.Create(cci.FunctionPointer, new DGetDefaultAudioEndpoint(GetDefaultAudioEndpoint),
                    this);
                hook.ThreadACL.SetExclusiveACL(new int[] {});

                try
                {
                    while (!Interface.CanUnload())
                    {
                        Thread.Sleep(1000);
                    }

                    hook.Dispose();
                }
                catch (Exception e)
                {
                    try
                    {
                        Interface.ReportError(RemoteHooking.GetCurrentProcessId(), e);
                    }
                    catch
                    {
                        //.NET Remoting timeout etc...
                    }
                }
                finally
                {
                    hook.Dispose();
                }
            }

            private static int GetDefaultAudioEndpoint(IMMDeviceEnumerator self, EDataFlow dataflow, ERole eRole,
                out IntPtr ppendpoint)
            {
                var entryPoint = HookRuntimeInfo.Callback as EntryPoint;

                if (entryPoint == null || entryPoint.Interface == null)
                    return self.GetDefaultAudioEndpoint(dataflow, eRole, out ppendpoint);

                var remoteInterface = entryPoint.Interface;

                try
                {
                    var devId = remoteInterface.GetDefaultDevice(dataflow, eRole);
                    return self.GetDevice(devId, out ppendpoint);
                }
                catch (Exception ex)
                {
                    remoteInterface.ReportError(RemoteHooking.GetCurrentProcessId(), ex);
                    //Something failed so return the actual default device
                    return self.GetDefaultAudioEndpoint(dataflow, eRole, out ppendpoint);
                }
            }
        }

        public class RemoteInterface : MarshalByRefObject, IRemoteHook
        {
            public Func<EDataFlow, ERole, string> SystemId
            {
                get;
                set;
            }

            public Func<bool> Unload
            {
                get;
                set;
            }

            public Action<int, Exception> ErrorHandler
            {
                get;
                set;
            }

            public bool CanUnload()
            {
                if (Unload == null)
                    return true;

                return Unload();
            }

            public void ReportError(int processId, Exception e)
            {
                if (ErrorHandler != null)
                    ErrorHandler(processId, e);
            }

            public string GetDefaultDevice(EDataFlow eDataFlow, ERole eRole)
            {
                if (SystemId == null)
                    return String.Empty;

                return SystemId(eDataFlow, eRole);
            }
        }
    }
}