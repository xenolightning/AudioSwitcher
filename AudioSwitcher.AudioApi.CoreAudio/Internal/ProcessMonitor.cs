using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public static class ProcessMonitor
    {
        public static IObservable<int> ProcessTerminated
        {
            get
            {
                return _processTerminated;
            }
        }

        private static Timer _processExitTimer;
        private static IEnumerable<int> _lastProcesses = new List<int>();
        private static AsyncBroadcaster<int> _processTerminated;

        static ProcessMonitor()
        {
            _processTerminated = new AsyncBroadcaster<int>();
            _processExitTimer = new Timer(ProcessTerminatedCheck, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private static void ProcessTerminatedCheck(object state)
        {
            var currentProcesses = Process.GetProcesses().Select(x => x.Id).ToList();
            var removed = _lastProcesses.Except(currentProcesses).ToList();

            foreach (var removedProcess in removed)
            {
                _processTerminated.OnNext(removedProcess);
            }

            _lastProcesses = currentProcesses;
        }
    }
}
