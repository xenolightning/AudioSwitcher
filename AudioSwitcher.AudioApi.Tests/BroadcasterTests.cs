using System;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class BroadcasterTests
    {
        [Fact]
        public void Broadcaster_Empty()
        {
            var b = new Broadcaster<int>();

            Assert.False(b.IsDisposed);
            Assert.False(b.HasObservers);
        }

        [Fact]
        public void Broadcaster_Dispose()
        {
            var b = new Broadcaster<int>();
            b.Dispose();
            Assert.True(b.IsDisposed);
        }

        [Fact]
        public void Broacaster_Disposed_HasObservers()
        {
            var b = new Broadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            b.Dispose();

            Assert.False(b.HasObservers);
        }

        [Fact]
        public void Broacaster_SubscriptionDisposed_HasObservers()
        {
            var b = new Broadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            sub.Dispose();

            Assert.False(b.HasObservers);
        }

        [Fact]
        public void Broacaster_Subscribe()
        {
            var b = new Broadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);
        }

        [Fact]
        public void Broacaster_Subscribe_HasObservers()
        {
            var b = new Broadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            Assert.True(b.HasObservers);
        }

        [Fact]
        public void Broacaster_OnNext()
        {
            var b = new Broadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);

            b.OnNext(2);

            Assert.NotEqual(-1, result);
            Assert.Equal(2, result);
        }

        [Fact]
        public void Broacaster_Subscribe_SubscriptionDispose()
        {
            var b = new Broadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            Assert.True(b.HasObservers);

            sub.Dispose();

            Assert.False(b.HasObservers);
        }

        [Fact]
        public void Broacaster_DisposedSubscription_OnNext()
        {
            var b = new Broadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);
            sub.Dispose();

            b.OnNext(2);

            Assert.NotEqual(2, result);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void Broacaster_Disposed_OnNext()
        {
            var b = new Broadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);
            sub.Dispose();

            b.Dispose();

            b.OnNext(2);

            Assert.NotEqual(2, result);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void Broacaster_OnCompleted()
        {
            var b = new Broadcaster<int>();

            bool complete = false;

            var sub = b.Subscribe(x => { }, () => { complete = true; });

            Assert.NotNull(sub);

            b.OnCompleted();

            Assert.True(complete);
        }

        [Fact]
        public void Broacaster_Dispose_OnCompleted()
        {
            var b = new Broadcaster<int>();

            bool complete = false;

            var sub = b.Subscribe(x => { }, () => { complete = true; });

            Assert.NotNull(sub);

            b.Dispose();

            Assert.True(complete);
        }

        [Fact]
        public void Broacaster_DisposedSubscription_OnCompleted()
        {
            var b = new Broadcaster<int>();

            bool complete = false;

            var sub = b.Subscribe(x => { }, () => { complete = true; });

            Assert.NotNull(sub);

            sub.Dispose();

            //disposing sub should not fire on completed
            Assert.False(complete);
        }

        [Fact]
        public void Broacaster_OnError_FromOnNext()
        {
            var b = new Broadcaster<int>();

            var exception = new Exception("HAI");
            Exception result = null;

            var sub = b.Subscribe(x => { throw exception; }, x => result = x);

            Assert.NotNull(sub);

            b.OnNext(1);

            Assert.NotNull(result);

            Assert.Equal(exception, result);
        }

        [Fact]
        public void Broacaster_OnError_FromOnError()
        {
            var b = new Broadcaster<int>();

            var exception = new Exception("HAI");
            Exception result = null;

            var sub = b.Subscribe(x => { }, x => result = x);

            Assert.NotNull(sub);

            b.OnError(exception);

            Assert.NotNull(result);

            Assert.Equal(exception, result);
        }

    }
}
