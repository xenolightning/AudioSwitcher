using System;
using System.Threading;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class AsyncBroadcasterTests
    {
        [Fact]
        public void AsyncBroadcaster_Empty()
        {
            var b = new AsyncBroadcaster<int>();

            Assert.False(b.IsDisposed);
            Assert.False(b.HasObservers);
        }

        [Fact]
        public void AsyncBroadcaster_Dispose()
        {
            var b = new AsyncBroadcaster<int>();
            b.Dispose();
            Assert.True(b.IsDisposed);
        }

        [Fact]
        public void AsyncBroacaster_Disposed_HasObservers()
        {
            var b = new AsyncBroadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            b.Dispose();

            Assert.False(b.HasObservers);
        }

        [Fact]
        public void AsyncBroacaster_SubscriptionDisposed_HasObservers()
        {
            var b = new AsyncBroadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            sub.Dispose();

            Assert.False(b.HasObservers);
        }

        [Fact]
        public void AsyncBroacaster_Subscribe()
        {
            var b = new AsyncBroadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);
        }

        [Fact]
        public void AsyncBroacaster_Subscribe_HasObservers()
        {
            var b = new AsyncBroadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            Assert.True(b.HasObservers);
        }

        [Fact]
        public void AsyncBroacaster_OnNext()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            int result = -1;

            var sub = b.Subscribe(x =>
            {
                result = x;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            b.OnNext(2);
            resetEvent.WaitOne();

            Assert.NotEqual(-1, result);
            Assert.Equal(2, result);
        }

        [Fact]
        public void AsyncBroacaster_Subscribe_SubscriptionDispose()
        {
            var b = new AsyncBroadcaster<int>();

            var sub = b.Subscribe(x => { });

            Assert.NotNull(sub);

            Assert.True(b.HasObservers);

            sub.Dispose();

            Assert.False(b.HasObservers);
        }

        [Fact]
        public void AsyncBroacaster_DisposedSubscription_OnNext()
        {
            var b = new AsyncBroadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);
            sub.Dispose();

            b.OnNext(2);

            Assert.NotEqual(2, result);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void AsyncBroacaster_Disposed_OnNext()
        {
            var b = new AsyncBroadcaster<int>();

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
        public void AsyncBroacaster_Disposed_Does_Not_Fire_OnNext()
        {
            var b = new AsyncBroadcaster<int>();

            int result = -1;

            var sub = b.Subscribe(x => { result = x; });

            Assert.NotNull(sub);

            b.Dispose();
            b.OnNext(2);

            Assert.NotEqual(2, result);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void AsyncBroacaster_OnCompleted()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            bool complete = false;

            var sub = b.Subscribe(x => { }, () =>
            {
                complete = true;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            b.OnCompleted();
            resetEvent.WaitOne();

            Assert.True(complete);
        }

        [Fact]
        public void AsyncBroacaster_Dispose_Does_Not_Fire_OnCompleted()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            int count = 0;

            var sub = b.Subscribe(x => { }, () =>
            {
                count++;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            //Dispose will call complete once
            b.Dispose();
            resetEvent.WaitOne();

            //ensure it's not called again
            resetEvent.Reset();
            b.OnCompleted();
            resetEvent.WaitOne(200);

            Assert.Equal(1, count);
        }

        [Fact]
        public void AsyncBroacaster_Disposed_OnCompleted()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            bool complete = false;

            var sub = b.Subscribe(x => { }, () =>
            {
                complete = true;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            b.Dispose();
            resetEvent.WaitOne();

            Assert.True(complete);
        }

        [Fact]
        public void AsyncBroacaster_DisposedSubscription_OnCompleted()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            bool complete = false;

            var sub = b.Subscribe(x => { }, () =>
            {
                complete = true;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            sub.Dispose();
            resetEvent.WaitOne(200);

            //disposing sub should not fire on completed
            Assert.False(complete);
        }

        [Fact]
        public void AsyncBroacaster_OnError_FromOnNext()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            var exception = new Exception("HAI");
            Exception result = null;

            var sub = b.Subscribe(x => { throw exception; }, x =>
            {
                result = x;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            b.OnNext(1);
            resetEvent.WaitOne();

            Assert.NotNull(result);

            Assert.Equal(exception, result);
        }

        [Fact]
        public void AsyncBroacaster_OnError_FromOnError()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            var exception = new Exception("HAI");
            Exception result = null;

            var sub = b.Subscribe(x => { }, x =>
            {
                result = x;
                resetEvent.Set();
            });

            Assert.NotNull(sub);

            b.OnError(exception);
            resetEvent.WaitOne();

            Assert.NotNull(result);

            Assert.Equal(exception, result);
        }

        [Fact]
        public void AsyncBroacaster_Disposed_Does_Not_Fire_OnError()
        {
            var b = new AsyncBroadcaster<int>();
            var resetEvent = new ManualResetEvent(false);

            var exception = new Exception("HAI");
            Exception result = null;

            var sub = b.Subscribe(x => { }, x =>
            {
                result = x;
            });

            Assert.NotNull(sub);

            b.Dispose();
            resetEvent.WaitOne(200);

            b.OnError(exception);
            resetEvent.WaitOne(200);

            Assert.Null(result);
        }


        [Fact]
        public void DelegateDisposable_Create()
        {
            int count = 0;
            var disposable = new DelegateDisposable(() => count = 1);

            Assert.NotNull(disposable);
            Assert.IsAssignableFrom<IDisposable>(disposable);

            disposable.Dispose();

            Assert.Equal(1, count);
        }

        [Fact]
        public void AsyncBroadcaster_Completed_Subscribe_Does_Not_Add_Observer()
        {
            var b = new AsyncBroadcaster<int>();
            b.OnCompleted();

            b.Subscribe(x => { });

            Assert.True(b.IsComplete);
            Assert.False(b.HasObservers);
        }

        [Fact]
        public void AsyncBroadcaster_Disposed_Subscribe_Throws_Exception()
        {
            var b = new AsyncBroadcaster<int>();
            b.Dispose();

            Assert.True(b.IsComplete);
            Assert.True(b.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() => b.Subscribe(x => { }));

            Assert.False(b.HasObservers);
        }
    }
}
