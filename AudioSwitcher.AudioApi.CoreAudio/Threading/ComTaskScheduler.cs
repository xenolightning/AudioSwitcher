using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.CoreAudio.Threading
{
    internal sealed class ComTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly List<Thread> _threads;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly BlockingCollection<Task> _tasks;
        public readonly List<int> ThreadIds;

        public override int MaximumConcurrencyLevel => _threads.Count;

        public ComTaskScheduler(int numberOfThreads)
        {
            if (numberOfThreads < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfThreads));

            // Initialize the tasks collection
            _tasks = new BlockingCollection<Task>();
            _cancellationToken = new CancellationTokenSource();

            // Create the threads to be used by this scheduler
            _threads = Enumerable.Range(0, numberOfThreads).Select(i =>
            {
                var thread = new Thread(ThreadStart);
                thread.IsBackground = true;
                thread.SetApartmentState(ApartmentState.STA);
                return thread;
            }).ToList();

            // Start all of the threads
            _threads.ForEach(t => t.Start());

            ThreadIds = _threads.Select(x => x.ManagedThreadId).ToList();
        }

        public void Dispose()
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _tasks.CompleteAdding();
            _tasks.Dispose();
        }

        protected override void QueueTask(Task task)
        {
            ThrowIfDisposed();

            _tasks.Add(task, _cancellationToken.Token);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            ThrowIfDisposed();

            return _tasks.ToArray();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            ThrowIfDisposed();

            if (!ThreadIds.Contains(Thread.CurrentThread.ManagedThreadId))
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

        private void ThrowIfDisposed()
        {
            if (_cancellationToken.IsCancellationRequested || _tasks.IsAddingCompleted)
                throw new ObjectDisposedException(typeof(ComTaskScheduler).Name);
        }
    }
}