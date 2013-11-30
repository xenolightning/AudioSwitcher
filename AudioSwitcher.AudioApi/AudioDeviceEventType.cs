namespace AudioSwitcher.AudioApi
{
    public enum AudioDeviceEventType
    {
        DefaultDevice,
        DefaultCommunicationsDevice,
        Added,
        Removed,
        Volume,
        Level,
        PropertyChanged,
        StateChanged
    }
}