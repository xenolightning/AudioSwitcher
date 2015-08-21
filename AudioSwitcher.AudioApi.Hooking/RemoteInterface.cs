using System;
using AudioSwitcher.AudioApi.Hooking.ComObjects;

namespace AudioSwitcher.AudioApi.Hooking
{
    public class RemoteInterface : MarshalByRefObject, IRemoteHook
    {
        public Func<DataFlow, Role, string> SystemId
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

        public string GetDefaultDevice(DataFlow dataFlow, Role role)
        {
            if (SystemId == null)
                return String.Empty;

            return SystemId(dataFlow, role);
        }
    }
}