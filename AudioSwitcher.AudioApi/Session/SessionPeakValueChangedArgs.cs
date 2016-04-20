namespace AudioSwitcher.AudioApi.Session
{
    public class SessionPeakValueChangedArgs
    {
        public IAudioSession Session { get; private set; }

        public double PeakValue { get; private set; }

        public SessionPeakValueChangedArgs(IAudioSession session, double peakValue)
        {
            Session = session;
            PeakValue = peakValue;
        }
    }
}