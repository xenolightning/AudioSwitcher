namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface IMicrophoneBoost : IDeviceCapability
    {
        int Level { get; }

        bool IsValidLevel(int level);

        int[] GetValidLevels();

    }
}