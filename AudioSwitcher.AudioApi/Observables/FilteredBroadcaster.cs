using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public sealed class FilteredBroadcaster<T> : Broadcaster<T>
    {
        private readonly IDisposable _observerSubscription;
        private readonly Func<T, bool> _predicate;

        internal FilteredBroadcaster(IObservable<T> observable, Func<T, bool> predicate)
        {
            _observerSubscription = observable.Subscribe(this);
            _predicate = predicate;
        }

        public override void OnNext(T value)
        {
            if (_predicate(value))
                base.OnNext(value);
        }

        protected override void Dispose(bool disposing)
        {
            _observerSubscription.Dispose();
        }
    }
}