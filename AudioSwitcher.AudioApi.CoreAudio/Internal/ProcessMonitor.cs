using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public static class ProcessMonitor
    {
        private static Timer _processExitTimer;
        private static IEnumerable<int> _lastProcesses = new List<int>();
        private static readonly Broadcaster<int> _processTerminated;

        public static IObservable<int> ProcessTerminated
        {
            get
            {
                return _processTerminated.AsObservable();
            }
        }

        static ProcessMonitor()
        {
            _processTerminated = new Broadcaster<int>();
            _processExitTimer = new Timer
            {
                Interval = 1000,
                AutoReset = false
            };

            _processExitTimer.Elapsed += TimerTick;
            _processExitTimer.Start();
        }

        private static void TimerTick(object sender, ElapsedEventArgs e)
        {
            var currentProcesses = Process.GetProcesses().Select(x => x.Id).ToList();
            var removed = _lastProcesses.Except(currentProcesses).ToList();

            foreach (var removedProcess in removed)
            {
                _processTerminated.OnNext(removedProcess);
            }

            _lastProcesses = currentProcesses;

            if (_processExitTimer != null)
                _processExitTimer.Start();
        }
    }
}
