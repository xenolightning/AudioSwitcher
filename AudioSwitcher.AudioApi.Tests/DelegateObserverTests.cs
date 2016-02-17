using System;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class DelegateObserverTests
    {

        [Fact]
        public void DelegateObserver_Constructor_Throws_For_Null_OnNext()
        {
            Assert.Throws<ArgumentNullException>(() => new Broadcaster<int>().Subscribe(onNext: null));
        }

        [Fact]
        public void DelegateObserver_Constructor_Throws_For_Null_OnComplete()
        {
            Assert.Throws<ArgumentNullException>(() => new Broadcaster<int>().Subscribe(x => { }, onCompleted: null));
        }

        [Fact]
        public void DelegateObserver_Constructor_Throws_For_Null_OnError()
        {
            Assert.Throws<ArgumentNullException>(() => new Broadcaster<int>().Subscribe(x => { }, null, () => { }));
        }

        [Fact]
        public void DelegateObserver_Disposed_Does_Not_Send_OnNext()
        {
            int result = -1;

            var b = new DelegateObserver<int>(
                i =>
                {
                    result = 1;
                },
                exception =>
                {

                },
                () =>
                {

                });

            b.Dispose();

            Assert.True(b.IsDisposed);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void DelegateObserver_Disposed_Does_Not_Send_OnError()
        {
            int result = -1;

            var b = new DelegateObserver<int>(
                i =>
                {
                },
                exception =>
                {
                    result = 1;
                },
                () =>
                {

                });

            b.Dispose();

            Assert.True(b.IsDisposed);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void DelegateObserver_Disposed_Does_Not_Send_OnComplete()
        {
            int result = -1;

            var b = new DelegateObserver<int>(
                i =>
                {
                },
                exception =>
                {
                },
                () =>
                {
                    result = 1;
                });

            b.Dispose();

            Assert.True(b.IsDisposed);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void DelegateObserver_Disposed_Does_Not_Fire_OnNext()
        {
            var b = new Broadcaster<int>();
            int result = 0;

            var sub = b.Subscribe(x =>
            {
                result = x;
            });

            sub.Dispose();

            b.OnNext(5);

            Assert.Equal(0, result);
        }

        [Fact]
        public void DelegateObserver_Disposed_Does_Not_Fire_OnComplete()
        {
            var b = new Broadcaster<int>();
            int result = 0;

            var sub = b.Subscribe(x => { }, () =>
              {
                  result = 5;
              });

            sub.Dispose();

            b.OnCompleted();

            Assert.Equal(0, result);
        }

        [Fact]
        public void DelegateObserver_Disposed_Does_Not_Fire_OnError()
        {
            var b = new Broadcaster<int>();
            int result = 0;

            var sub = b.Subscribe(x => { }, x =>
            {
                result = 5;
            });

            sub.Dispose();

            b.OnError(new Exception());

            Assert.Equal(0, result);
        }

        [Fact]
        public void Observable_AsObservable_Null_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                (null as IObservable<int>).AsObservable();
            });
        }

    }
}
