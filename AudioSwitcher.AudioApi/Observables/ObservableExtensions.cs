using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public static class ObservableExtensions
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => { throw ex; };

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(new DelegateObserver<T>(onNext));
        }
    }
}
