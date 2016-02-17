using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.CoreAudio.Threading
{
    internal sealed class ComTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly BlockingCollection<Task> _tasks;

        public int ThreadId => _thread?.ManagedThreadId ?? -1;

        public override int MaximumConcurrencyLevel => 1;

        public ComTaskScheduler()
        {
            _tasks = new BlockingCollection<Task>();
            _cancellationToken = new CancellationTokenSource();

            _thread = new Thread(ThreadStart)
            {
                IsBackground = true
            };
            _thread.TrySetApartmentState(ApartmentState.STA);

            _thread.Start();
        }

        public void Dispose()
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            _cancellationToken.Cancel();
            _tasks.CompleteAdding();
        }

        protected override void QueueTask(Task task)
        {
            VerifyNotDisposed();

            _tasks.Add(task, _cancellationToken.Token);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            VerifyNotDisposed();

            return _tasks.ToArray();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            VerifyNotDisposed();

            if (_thread != Thread.CurrentThread)
                return false;

            if (_cancellationToken.Token.IsCancellationRequested)
                return false;

            return TryExecuteTask(task);
        }

        private void ThreadStart()
        {
            var token = _cancellationToken.Token;

            foreach (var task in _tasks.GetConsumingEnumerable(token))
                TryExecuteTask(task);
        }

        private void VerifyNotDisposed()
        {
            if (_cancellationToken.IsCancellationRequested || _tasks.IsAddingCompleted)
                throw new ObjectDisposedException(typeof(ComTaskScheduler).Name);
        }
    }
}