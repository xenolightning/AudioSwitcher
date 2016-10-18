using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public sealed class DelegateDisposable : IDisposable
    {
        private readonly Action _disposeAction;

        public DelegateDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction();
        }
    }
}