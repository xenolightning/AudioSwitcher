using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public sealed class AsyncDispatcher<T> : DispatcherBase<T>
    {
        public override bool HasObservers
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsDisposed
        {
            get { throw new NotImplementedException(); }
        }

        public override void OnNext(T value)
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
