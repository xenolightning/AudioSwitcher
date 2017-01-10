using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.CoreAudio.Threading
{
    internal static class ComThread
    {
        private static readonly ComTaskScheduler ComScheduler = new ComTaskScheduler(1);

        private static bool InvokeRequired => !Scheduler.ThreadIds.Contains(Thread.CurrentThread.ManagedThreadId);

        private static ComTaskScheduler Scheduler => ComScheduler;

        /// <summary>
        /// Asserts that the execution following this statement is running on the ComThreads
        /// <exception cref="InvalidThreadException">Thrown if the assertion fails</exception>
        /// </summary>
        public static void Assert()
        {
            if (InvokeRequired)
                throw new InvalidThreadException("This operation must be run on a STA COM Thread");
        }

        public static void Invoke(Action action)
        {
            if (!InvokeRequired)
            {
                action();
                return;
            }

            BeginInvoke(action).GetAwaiter().GetResult();
        }

        public static Task BeginInvoke(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, ComScheduler);
        }

        public static T Invoke<T>(Func<T> func)
        {
            if (!InvokeRequired)
                return func();

            return BeginInvoke(func).GetAwaiter().GetResult();
        }

        public static Task<T> BeginInvoke<T>(Func<T> func)
        {
            return Task<T>.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, ComScheduler);
        }
    }
}