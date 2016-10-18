using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public static class ObservableExtensions
    {
        private static readonly Action Nop = () => { };
        private static readonly Action<Exception> Throw = ex => { throw ex; };

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(new DelegateObserver<T>(onNext, Throw, Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext,
            Action<Exception> onError)
        {
            return observable.Subscribe(new DelegateObserver<T>(onNext, onError, Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action onCompleted)
        {
            return observable.Subscribe(new DelegateObserver<T>(onNext, Throw, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext,
            Action<Exception> onError, Action onCompleted)
        {
            return observable.Subscribe(new DelegateObserver<T>(onNext, onError, onCompleted));
        }

        public static IObservable<T> When<T>(this IObservable<T> observable, Func<T, bool> predicate)
        {
            return new FilteredBroadcaster<T>(observable, predicate);
        }

        public static IObservable<T> AsObservable<T>(this IObservable<T> observable)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable));

            return observable;
        }
    }
}