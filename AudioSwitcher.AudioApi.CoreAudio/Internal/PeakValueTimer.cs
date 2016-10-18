using System;
using System.Timers;
using AudioSwitcher.AudioApi.Observables;
using Timer = System.Timers.Timer;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PeakValueTimer
    {
        private static readonly Timer _peakValueTimer;
        private static readonly Broadcaster<long> _peakValueTick;

        public static IObservable<long> PeakValueTick => _peakValueTick;

        static PeakValueTimer()
        {

            _peakValueTick = new Broadcaster<long>();
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
