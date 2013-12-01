namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Delegate for the events on an audio device
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AudioDeviceChangedHandler(object sender, AudioDeviceChangedEventArgs e);
}