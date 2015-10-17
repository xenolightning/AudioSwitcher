namespace AudioSwitcher.AudioApi.Hooking
{
    public interface IHook
    {
        EHookStatus Status { get; }

        /// <summary>
        /// Hooks the specified process. Returns true if the hook was successful
        /// </summary>
        /// <param name="processId"></param>
        /// <returns>true if the hook was successful</returns>
        bool Hook(int processId);

        /// <summary>
        /// Attempts to Unhook the process that is hooked by this instance. Returns true if unhook was successful
        /// </summary>
        /// <returns>true if unhook was successful</returns>
        bool UnHook();
    }

    public enum EHookStatus
    {
        Inactive,
        Pending,
        Active
    }
}