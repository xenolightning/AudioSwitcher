using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public interface IDispatcher<T> : IDispatcher<T, T>
    {
    }

    public interface IDispatcher<in TSource, out TResult> : IObserver<TSource>, IObservable<TResult>
    {
    }
}
