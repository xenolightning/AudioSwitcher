using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class ProcessMonitor
    {

        private static readonly Timer _processExitTimer;
        private static readonly Broadcaster<int> _processTerminated;
        private static IEnumerable<int> _lastProcesses = new List<int>();

        public static IObservable<int> ProcessTerminated => _processTerminated.AsObservable();

        static ProcessMonitor()
        {
            _processTerminated = new Broadcaster<int>();
            _processExitTimer = new Timer
            {
                Interval = 1500,
                AutoReset = false
            };

            _processExitTimer.Elapsed += TimerTick;
            _processExitTimer.Start();
        }

        private static void TimerTick(object sender, ElapsedEventArgs e)
        {
            var processIds = Process.GetProcesses().Select(x => x.Id).ToList();

            foreach (var removedProcess in _lastProcesses.Except(processIds))
            {
                _processTerminated.OnNext(removedProcess);
            }

            _lastProcesses = processIds;

            _processExitTimer?.Start();
        }
    }
}
