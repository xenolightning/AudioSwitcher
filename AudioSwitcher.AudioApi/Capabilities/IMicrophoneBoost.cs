namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface IMicrophoneBoost
    {
        int Level { get; }

        bool IsValidLevel(int level);

        int[] GetValidLevels();

    }
}