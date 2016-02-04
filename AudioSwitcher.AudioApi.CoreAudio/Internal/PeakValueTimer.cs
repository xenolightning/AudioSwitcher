using System;
using System.Threading;
using System.Timers;
using AudioSwitcher.AudioApi.Observables;
using Timer = System.Timers.Timer;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public static class PeakValueTimer
    {
        private static readonly Timer _peakValueTimer;
        private static readonly Broadcaster<long> _peakValueTick;

        public static IObservable<long> PeakValueTick
        {
            get
            {
                return _peakValueTick;
            }
        }

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
            _peakValueTimer.Enabled = false;
            //Console.WriteLine("{0} - START - {1}", Thread.CurrentThread.ManagedThreadId, e.SignalTime.ToString("HH:mm:ss:ffff"));
            _peakValueTick.OnNext(Environment.TickCount);
            //Console.WriteLine("{0} - END - {1}", Thread.CurrentThread.ManagedThreadId, e.SignalTime.ToString("HH:mm:ss:ffff"));
            _peakValueTimer.Enabled = true;
        }
    }
}
