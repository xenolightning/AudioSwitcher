namespace AudioSwitcher.AudioApi
{
    /// <summary>
    /// Common sample rates. Defining an enum so it's easier to control valid values
    /// </summary>
    public enum SampleRate
    {
        R44100 = 44100,
        R48000 = 48000,
        R88200 = 88200,
        R96000 = 96000,
        R192000 = 192000,
    }
}