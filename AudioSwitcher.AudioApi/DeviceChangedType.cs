namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     The type of change raised
    /// </summary>
    public enum DeviceChangedType
    {
        DefaultDevice,
        DefaultCommunicationsDevice,
        DeviceAdded,
        DeviceRemoved,
        PropertyChanged,
        StateChanged
    }
}