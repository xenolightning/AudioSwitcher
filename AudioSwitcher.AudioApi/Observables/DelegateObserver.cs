using System;
using System.Threading;

namespace AudioSwitcher.AudioApi.Observables
{
    internal sealed class DelegateObserver<T> : IObserver<T>, IDisposable
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        private int _isStopped;

        public DelegateObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onError == null) throw new ArgumentNullException("onError");
            if (onCompleted == null) throw new ArgumentNullException("onCompleted");

            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
            if (_isStopped == 0)
                _onNext(value);
        }

        public void OnError(Exception error)
        {
            if (Interlocked.Exchange(ref _isStopped, 1) == 0)
                _onError(error);
        }

        public void OnCompleted()
        {
            if (Interlocked.Exchange(ref _isStopped, 1) == 0)
                _onCompleted();
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _isStopped, 1);
        }

    }
}
