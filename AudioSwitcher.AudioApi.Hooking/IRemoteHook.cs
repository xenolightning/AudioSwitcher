using System;

namespace AudioSwitcher.AudioApi.Hooking
{
    public interface IRemoteHook
    {

        /// <summary>
        /// Returns true if the hook should be unloaded from the target
        /// </summary>
        /// <returns></returns>
        bool CanUnload();


        void ReportError(int processId, Exception e);

    }
}