namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     The type of change raised
    /// </summary>
    public enum DeviceChangedType
    {
        DefaultChanged,
        DeviceAdded,
        DeviceRemoved,
        PropertyChanged,
        StateChanged,
        MuteChanged,
        VolumeChanged,
        PeakValueChanged
    }
}