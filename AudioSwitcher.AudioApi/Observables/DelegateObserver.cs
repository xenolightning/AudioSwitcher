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

        public DelegateObserver(Action<T> onNext, Action<Exception> onError)
            :this(onNext, onError, ObservableExtensions.Nop)
        {
        }

        public DelegateObserver(Action<T> onNext, Action onCompleted)
            :this(onNext, ObservableExtensions.Throw, onCompleted)
        {
        }

        public DelegateObserver(Action<T> onNext)
            : this(onNext, ObservableExtensions.Throw, ObservableExtensions.Nop)
        {
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
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isStopped = 1;
            }
        }

        //internal DelegateObserverSafe<T> MakeSafe(IDisposable disposable)
        //{
        //    return new DelegateObserverSafe<T>(_onNext, _onError, _onCompleted, disposable);
        //}
    }
}
