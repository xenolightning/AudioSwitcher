using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public static class PeakValueTimer
    {
        public static IObservable<long> PeakValueTick
        {
            get
            {
                return _peakValueTick;
            }
        }

        private static readonly Timer _peakValueTimer;
        private static readonly AsyncBroadcaster<long> _peakValueTick;

        static PeakValueTimer()
        {
            _peakValueTick = new AsyncBroadcaster<long>();
            _peakValueTimer = new Timer
            {
                Interval = 50,
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
