using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class AsyncManualResetEvent
    {
        private readonly object _sync;

        private TaskCompletionSource<object> _tcs;

        public AsyncManualResetEvent(bool set)
        {
            _sync = new object();
            _tcs = new TaskCompletionSource<object>();
            if (set)
                _tcs.SetResult(null);
        }

        public AsyncManualResetEvent()
            : this(false)
        {
        }

        public bool IsSet
        {
            get { lock (_sync) return _tcs.Task.IsCompleted; }
        }

        public Task WaitAsync()
        {
            lock (_sync)
            {
                return _tcs.Task;
            }
        }

        public void Wait()
        {
            WaitAsync().Wait();
        }

        public void Wait(CancellationToken cancellationToken)
        {
            var ret = WaitAsync();
            if (ret.IsCompleted)
                return;

            ret.Wait(cancellationToken);
        }

        public void Set()
        {
            lock (_sync)
            {
                _tcs.TrySetResult(null);
            }
        }

        public void Reset()
        {
            lock (_sync)
            {
                if (_tcs.Task.IsCompleted)
                    _tcs = new TaskCompletionSource<object>();
            }
        }
    }
}
