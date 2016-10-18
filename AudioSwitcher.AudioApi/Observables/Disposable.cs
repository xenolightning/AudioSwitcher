using System;

namespace AudioSwitcher.AudioApi.Observables
{
    public static class Disposable
    {
        public static IDisposable Empty => DefaultDisposable.Instance;
    }
}