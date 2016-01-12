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
        public void Broacaster_Disposed_OnCompleted()
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

        [Fact]
        public void Broacaster_Disposed_Does_Not_Fire_OnError()
        {
            var b = new Broadcaster<int>();

            var exception = new Exception("HAI");
            Exception result = null;

            var sub = b.Subscribe(x => { }, x => result = x);

            Assert.NotNull(sub);

            b.Dispose();
            b.OnError(exception);

            Assert.Null(result);
        }

        [Fact]
        public void Broacaster_Disposed_Does_Not_Fire_OnNext()
        {
            var b = new Broadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);

            b.Dispose();
            b.OnNext(2);

            Assert.NotEqual(2, result);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void Broacaster_Disposed_Does_Not_Fire_OnCompleted()
        {
            var b = new Broadcaster<int>();

            int count = 0;

            var sub = b.Subscribe(x => { }, () => { count++; });

            Assert.NotNull(sub);

            //Dispose will call complete once
            b.Dispose();

            //ensure it's not called again
            b.OnCompleted();

            Assert.Equal(1, count);
        }

        [Fact]
        public void Broacaster_Completed_Does_Not_Fire_OnNext()
        {
            var b = new Broadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);

            b.OnCompleted();
            b.OnNext(2);

            Assert.NotEqual(2, result);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void Broacaster_Completed_Does_Not_Fire_OnCompleted()
        {
            var b = new Broadcaster<int>();

            int count = 0;

            var sub = b.Subscribe(x => { }, () => { count++; });

            Assert.NotNull(sub);

            //Dispose will call complete once
            b.OnCompleted();

            //ensure it's not called again
            b.OnCompleted();

            Assert.Equal(1, count);
        }


        [Fact]
        public void DelegateDisposable_Create()
        {
            int count = 0;
            var disposable = DelegateDisposable.Create(() => count = 1);

            Assert.NotNull(disposable);
            Assert.IsAssignableFrom<IDisposable>(disposable);

            disposable.Dispose();

            Assert.Equal(1, count);
        }
    }
}
