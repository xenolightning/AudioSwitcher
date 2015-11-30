namespace AudioSwitcher.AudioApi.Session
{
    public sealed class PeakValueChangedArgs
    {
        public IAudioSession Session
        {
            get;
            private set;
        }

        public double PeakValue
        {
            get;
            private set;
        }

        public PeakValueChangedArgs(IAudioSession session, double peakValue)
        {
            Session = session;
            PeakValue = peakValue;
        }
    }
}