using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Observables
{
    public sealed class AsyncBroadcaster<T> : BroadcasterBase<T>
    {
        private readonly object _observerLock = new object();
        private readonly HashSet<IObserver<T>> _observers;
        private bool _isComplete;

        public override bool HasObservers
        {
            get
            {
                lock (_observerLock)
                {
                    return _observers.Count > 0;
                }
            }
        }

        public override bool IsComplete => _isComplete;

        public AsyncBroadcaster()
        {
            _observers = new HashSet<IObserver<T>>();
        }

        public override void OnNext(T value)
        {
            if (IsDisposed || IsComplete)
                return;

            RaiseAllObserversAsync(x =>
            {
                try
                {
                    x.OnNext(value);
                }
                catch (Exception ex)
                {
                    x.OnError(ex);
                }
            });
        }

        public override void OnError(Exception error)
        {
            if (IsDisposed)
                return;

            RaiseAllObserversAsync(x => x.OnError(error));
        }

        public override void OnCompleted()
        {
            if (IsDisposed || IsComplete)
                return;

            RaiseAllObserversAsync(x => x.OnCompleted());

            //Too bad if something dodgy happens, we consider ourselves complete now
            _isComplete = true;
        }

        private void RaiseAllObserversAsync(Action<IObserver<T>> observerAction)
        {
            List<IObserver<T>> lObservers;
            lock (_observerLock)
            {
                lObservers = _observers.ToList();
            }

            Parallel.ForEach(lObservers, observerAction);
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Observable is disposed");

            if (IsComplete)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            lock (_observerLock)
            {
                _observers.Add(observer);
            }

            return new DelegateDisposable(() =>
            {
                lock (_observerLock)
                {
                    _observers.Remove(observer);
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            OnCompleted();

            lock (_observerLock)
            {
                _observers.Clear();
            }
        }
    }
}