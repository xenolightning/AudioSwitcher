using System;

namespace AudioSwitcher.AudioApi.Observables
{
    internal sealed class DefaultDisposable : IDisposable
    {
        /// <summary>
        /// A disposable that does nothing. Used for empty returns
        /// </summary>
        public static readonly DefaultDisposable Instance = new DefaultDisposable();

        private DefaultDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}