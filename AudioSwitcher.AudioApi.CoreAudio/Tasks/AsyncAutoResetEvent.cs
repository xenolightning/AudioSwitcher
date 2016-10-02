using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;

// Original idea by Stephen Toub: http://blogs.msdn.com/b/pfxteam/archive/2012/02/11/10266923.aspx

// ReSharper disable once CheckNamespace
namespace Nito.AsyncEx
{
    /// <summary>
    /// An async-compatible auto-reset event.
    /// </summary>
    [DebuggerNonUserCode]
    internal sealed class AsyncAutoResetEvent
    {
        /// <summary>
        /// The queue of TCSs that other tasks are awaiting.
        /// </summary>
        private readonly IAsyncWaitQueue<object> _queue;

        /// <summary>
        /// The current state of the event.
        /// </summary>
        private bool _set;

        /// <summary>
        /// The object used for mutual exclusion.
        /// </summary>
        private readonly object _mutex;

        /// <summary>
        /// Creates an async-compatible auto-reset event.
        /// </summary>
        /// <param name="set">Whether the auto-reset event is initially set or unset.</param>
        /// <param name="queue">The wait queue used to manage waiters.</param>
        public AsyncAutoResetEvent(bool set, IAsyncWaitQueue<object> queue)
        {
            _queue = queue;
            _set = set;
            _mutex = new object();
            //if (set)
            //    Enlightenment.Trace.AsyncAutoResetEvent_Set(this);
        }

        /// <summary>
        /// Creates an async-compatible auto-reset event.
        /// </summary>
        /// <param name="set">Whether the auto-reset event is initially set or unset.</param>
        public AsyncAutoResetEvent(bool set)
            : this(set, new DefaultAsyncWaitQueue<object>())
        {
        }

        /// <summary>
        /// Creates an async-compatible auto-reset event that is initially unset.
        /// </summary>
        public AsyncAutoResetEvent()
          : this(false, new DefaultAsyncWaitQueue<object>())
        {
        }

        /// <summary>
        /// Whether this event is currently set. This member is seldom used; code using this member has a high possibility of race conditions.
        /// </summary>
        public bool IsSet
        {
            get { lock (_mutex) return _set; }
        }

        /// <summary>
        /// Asynchronously waits for this event to be set. If the event is set, this method will auto-reset it and return immediately, even if the cancellation token is already signalled. If the wait is canceled, then it will not auto-reset this event.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token used to cancel this wait.</param>
        public Task WaitAsync(CancellationToken cancellationToken)
        {
            Task ret;
            lock (_mutex)
            {
                if (_set)
                {
                    _set = false;
                    ret = TaskConstants.Completed;
                }
                else
                {
                    ret = _queue.Enqueue(_mutex, cancellationToken);
                }
                //Enlightenment.Trace.AsyncAutoResetEvent_TrackWait(this, ret);
            }

            return ret;
        }

        /// <summary>
        /// Synchronously waits for this event to be set. If the event is set, this method will auto-reset it and return immediately, even if the cancellation token is already signalled. If the wait is canceled, then it will not auto-reset this event. This method may block the calling thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token used to cancel this wait.</param>
        public void Wait(CancellationToken cancellationToken)
        {
            Task ret;
            lock (_mutex)
            {
                if (_set)
                {
                    _set = false;
                    return;
                }

                ret = _queue.Enqueue(_mutex, cancellationToken);
            }

            ret.WaitAndUnwrapException();
        }

        /// <summary>
        /// Asynchronously waits for this event to be set. If the event is set, this method will auto-reset it and return immediately.
        /// </summary>
        public Task WaitAsync()
        {
            return WaitAsync(CancellationToken.None);
        }

        public Task WaitAsync(int millis)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(millis);

            return WaitAsync(cts.Token);
        }

        /// <summary>
        /// Synchronously waits for this event to be set. If the event is set, this method will auto-reset it and return immediately. This method may block the calling thread.
        /// </summary>
        public void Wait()
        {
            Wait(CancellationToken.None);
        }

        public void Wait(int millis)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(millis);

            Wait(cts.Token);
        }

        /// <summary>
        /// Sets the event, atomically completing a task returned by <see cref="o:WaitAsync"/>. If the event is already set, this method does nothing.
        /// </summary>
        public void Set()
        {
            IDisposable finish = null;
            lock (_mutex)
            {
                //Enlightenment.Trace.AsyncAutoResetEvent_Set(this);
                if (_queue.IsEmpty)
                    _set = true;
                else
                    finish = _queue.Dequeue();
            }
            finish?.Dispose();
        }

    }
}
