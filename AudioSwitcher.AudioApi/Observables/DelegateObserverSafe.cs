namespace AudioSwitcher.AudioApi.Observables
{
    //internal sealed class DelegateObserverSafe<T> : IObserver<T>, IDisposable
    //{
    //    private readonly IDisposable _disposable;
    //    private Action<T> _onNext;
    //    private Action<Exception> _onError;
    //    private Action _onCompleted;

    //    public static IObserver<T> Create(IObserver<T> observer, IDisposable disposable)
    //    {
    //        var obs = observer as DelegateObserver<T>;
    //        if (obs != null)
    //            return obs.MakeSafe(disposable);

    //        return new DelegateObserverSafe<T>(observer, disposable);
    //    }

    //    public DelegateObserverSafe(Action<T> onNext, Action<Exception> onError, Action onCompleted, IDisposable disposable)
    //    {
    //        _onNext = onNext;
    //        _onError = onError;
    //        _onCompleted = onCompleted;
    //        _disposable = disposable;
    //    }

    //    private DelegateObserverSafe(IObserver<T> observer, IDisposable disposable)
    //        :this(observer.OnNext, observer.OnError, observer.OnCompleted, disposable)
    //    {
    //    }

    //    public void OnNext(T value)
    //    {
    //        var noError = false;
    //        try
    //        {
    //            _onNext(value);
    //            noError = true;
    //        }
    //        finally
    //        {
    //            if (!noError)
    //                _disposable.Dispose();
    //        }
    //    }

    //    public void OnError(Exception error)
    //    {
    //        try
    //        {
    //            _onError(error);
    //        }
    //        finally
    //        {
    //            _disposable.Dispose();
    //        }
    //    }

    //    public void OnCompleted()
    //    {
    //        try
    //        {
    //            _onCompleted();
    //        }
    //        finally
    //        {
    //            _disposable.Dispose();
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        _disposable.Dispose();
    //    }
    //}
}
