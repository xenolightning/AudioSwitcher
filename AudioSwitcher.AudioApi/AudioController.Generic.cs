namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Audio controller that can be used for strongly typed devices
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    public abstract class AudioController<TDevice> : AudioController
        where TDevice : AudioDevice
    {
        public new TDevice DefaultPlaybackDevice
        {
            get { return base.DefaultPlaybackDevice as TDevice; }
            protected set { base.DefaultPlaybackDevice = value; }
        }

        public new TDevice DefaultCommunicationsPlaybackDevice
        {
            get { return base.DefaultCommunicationsPlaybackDevice as TDevice; }
            protected set { base.DefaultCommunicationsPlaybackDevice = value; }
        }

        public new TDevice DefaultRecordingDevice
        {
            get { return base.DefaultPlaybackDevice as TDevice; }
            protected set { base.DefaultPlaybackDevice = value; }
        }

        public new TDevice DefaultCommunicationsRecordingDevice
        {
            get { return base.DefaultCommunicationsRecordingDevice as TDevice; }
            protected set { base.DefaultCommunicationsRecordingDevice = value; }
        }
    }
}