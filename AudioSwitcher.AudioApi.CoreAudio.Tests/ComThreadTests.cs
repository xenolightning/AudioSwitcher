using AudioSwitcher.AudioApi.CoreAudio.Threading;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    public class ComThreadTests
    {

        [Fact]
        public void ComThread_Assert_Throws()
        {

            var exception = Assert.Throws<InvalidThreadException>(() => ComThread.Assert());

            Assert.NotNull(exception.Message);

        }

    }
}
