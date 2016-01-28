using System;
using System.Timers;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public static class PeakValueTimer
    {
        private static readonly Timer _peakValueTimer;
        private static readonly AsyncBroadcaster<long> _peakValueTick;

        public static IObservable<long> PeakValueTick
        {
            get
            {
                return _peakValueTick;
            }
        }

        static PeakValueTimer()
        {
            _peakValueTick = new AsyncBroadcaster<long>();
            _peakValueTimer = new Timer
            {
                Interval = 100,
                AutoReset = false
            };

            _peakValueTimer.Elapsed += TimerTick;
            _peakValueTimer.Start();
        }

        private static void TimerTick(object sender, ElapsedEventArgs e)
        {
            _peakValueTick.OnNext(Environment.TickCount);
            _peakValueTimer.Start();
        }
    }
}
