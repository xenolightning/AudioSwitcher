using System;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class DisposableTests
    {

        [Fact]
        public void Disposable_Empty_Is_Disposable()
        {
            Assert.IsAssignableFrom<IDisposable>(Disposable.Empty);
        }

        [Fact]
        public void Disposable_Empty_Returns_Same_Instance()
        {
            var d1 = Disposable.Empty;
            var d2 = Disposable.Empty;

            Assert.Equal(d1, d2);
        }

        [Fact]
        public void Default_Disposable_Does_Nothing()
        {
            DefaultDisposable.Instance.Dispose();
        }

        [Fact]
        public void Default_Disposable_Can_Be_Called_Multiple_Times()
        {
            DefaultDisposable.Instance.Dispose();
            DefaultDisposable.Instance.Dispose();
            DefaultDisposable.Instance.Dispose();
            DefaultDisposable.Instance.Dispose();
        }

    }
}
