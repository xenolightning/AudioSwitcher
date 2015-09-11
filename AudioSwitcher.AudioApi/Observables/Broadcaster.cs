using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.AudioApi.Observables
{
    public sealed class Broadcaster<T> : BroadcasterBase<T>
    {
        private readonly HashSet<IObserver<T>> _observers;
        private readonly object _observerLock = new object();
        private bool _isDisposed;

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

        public override void OnNext(T value)
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
            if (IsDisposed)
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
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
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

        public override void Dispose()
        {
            OnCompleted();
            _isDisposed = true;

            lock (_observerLock)
            {
                _observers.Clear();
            }
        }
    }
}
