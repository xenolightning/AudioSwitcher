using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.AudioApi.Observables
{
    public class Broadcaster<T> : BroadcasterBase<T>
    {
        private readonly HashSet<IObserver<T>> _observers;
        private readonly object _observerLock = new object();
        private bool _isDisposed;
        private bool _isComplete;

        public Broadcaster()
        {
            _observers = new HashSet<IObserver<T>>();
        }

        public override bool HasObservers
        {
            get
            {
                return _observers.Count > 0;
            }
        }

        public override bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        public override bool IsComplete
        {
            get
            {
                return _isComplete;
            }
        }

        public override void OnNext(T value)
        {
            if (IsDisposed || IsComplete)
                return;

            IEnumerable<IObserver<T>> coll;
            lock (_observerLock)
            {
                coll = _observers.ToList();
            }

            foreach (var observer in coll)
            {
                try
                {
                    observer.OnNext(value);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            }
        }

        public override void OnError(Exception error)
        {
            if (IsDisposed)
                return;

            IEnumerable<IObserver<T>> coll;
            lock (_observerLock)
            {
                coll = _observers.ToList();
            }

            foreach (var observer in coll)
            {
                observer.OnError(error);
            }
        }

        public override void OnCompleted()
        {
            if (IsDisposed || IsComplete)
                return;

            IEnumerable<IObserver<T>> coll;
            lock (_observerLock)
            {
                coll = _observers.ToList();
            }

            foreach (var observer in coll)
            {
                observer.OnCompleted();
            }

            _isComplete = true;
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            if(IsDisposed)
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
                ObserverDisposal(observer);
            });
        }

        protected virtual void ObserverDisposal(IObserver<T> observer)
        {
            lock (_observerLock)
            {
                _observers.Remove(observer);
            }
        }

        public override void Dispose()
        {
            OnCompleted();

            lock (_observerLock)
            {
                _observers.Clear();
            }

            _isDisposed = true;
        }
    }
}
