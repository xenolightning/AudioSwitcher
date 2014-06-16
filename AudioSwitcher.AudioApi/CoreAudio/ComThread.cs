using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class ComThread
    {

        private static ComTaskScheduler _comScheduler = new ComTaskScheduler();

        public static bool InvokeRequired
        {
            get { return Thread.CurrentThread.ManagedThreadId != TaskScheduler.ThreadId; }
        }

        public static ComTaskScheduler TaskScheduler
        {
            get { return _comScheduler; }
        }

        public static void Invoke(Action action)
        {
            if (!InvokeRequired)
            {
                action();
                return;
            }

            BeginInvoke(action).Wait();
        }

        public static Task BeginInvoke(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _comScheduler);
        }

        public static T Invoke<T>(Func<T> func)
        {
            if (!InvokeRequired)
                return func();

            return BeginInvoke(func).Result;
        }

        public static Task<T> BeginInvoke<T>(Func<T> func)
        {
            return Task<T>.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, _comScheduler);
        }

    }
}
