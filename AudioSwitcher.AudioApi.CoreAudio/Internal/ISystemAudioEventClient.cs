namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal interface ISystemAudioEventClient
    {
        /// <summary>
        ///     Device State Changed
        /// </summary>
        void OnDeviceStateChanged(string deviceId, EDeviceState newState);

        /// <summary>
        ///     Device DeviceAdded
        /// </summary>
        void OnDeviceAdded(string pwstrDeviceId);

        /// <summary>
        ///     Device DeviceRemoved
        /// </summary>
        void OnDeviceRemoved(string deviceId);

        /// <summary>
        ///     Default Device Changed
        /// </summary>
        void OnDefaultDeviceChanged(EDataFlow flow, ERole role, string defaultDeviceId);

        /// <summary>
        ///     Property Value Changed
        /// </summary>
        /// <param name="pwstrDeviceId"></param>
        /// <param name="key"></param>
        void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key);
    }
}