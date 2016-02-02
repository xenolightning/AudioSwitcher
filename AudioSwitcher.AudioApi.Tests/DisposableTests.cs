using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
