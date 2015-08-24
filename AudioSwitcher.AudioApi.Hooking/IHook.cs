namespace AudioSwitcher.AudioApi.Hooking
{
    public interface IHook
    {
        EHookStatus Status { get; }

        void Hook(int processId);

        void UnHook();
    }

    public enum EHookStatus
    {
        Inactive,
        Pending,
        Active
    }
}