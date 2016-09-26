using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class FilteredBroadcasterTests
    {


        [Fact]
        public void Filtered_Broadcaster_Filters_Values()
        {
            var b = new Broadcaster<int>();
            var count = 0;

            b.When(x => x == 2).Subscribe(x =>
            {
                count++;
            });

            b.OnNext(1);
            b.OnNext(2);
            b.OnNext(3);
            b.OnNext(4);

            Assert.Equal(1, count);
        }

        [Fact]
        public void Filtered_Broadcaster_Filters_Values_Complex_Predicate()
        {
            var b = new Broadcaster<int>();
            var count = 0;

            b.When(x =>
            {
                if (x < 0)
                    return true;

                return x % 2 == 0;
            })
            .Subscribe(x =>
            {
                count++;
            });

            b.OnNext(-3); //y
            b.OnNext(-2); //y
            b.OnNext(-1); //y
            b.OnNext(0); //y
            b.OnNext(1); //n
            b.OnNext(2); //y
            b.OnNext(3); //n
            b.OnNext(4); //y
            b.OnNext(5); //n

            Assert.Equal(6, count);
        }

        [Fact]
        public void Filtered_Broadcaster_Completed_NotDisposed_Intiated_From_Source()
        {
            var b = new Broadcaster<int>();
            var count = 0;

            var fo = b.When(x => true) as FilteredBroadcaster<int>;

            fo.Subscribe(x =>
            {
                count++;
            });

            b.Dispose();

            b.OnNext(1);
            b.OnNext(1);
            b.OnNext(1);
            b.OnNext(1);

            Assert.NotNull(fo);
            Assert.False(fo.IsDisposed);
            Assert.True(fo.IsComplete);
            Assert.Equal(0, count);
        }
        [Fact]
        public void Filtered_Broadcaster_Dispose_Subscription()
        {
            var b = new Broadcaster<int>();

            var fo = b.When(x => true) as FilteredBroadcaster<int>;

            var sub  = fo.Subscribe(x =>
            {
            });

            sub.Dispose();
        }


        [Fact]
        public void Filtered_Broadcaster_Is_Disposed()
        {
            var b = new Broadcaster<int>().When(x => x == 2) as FilteredBroadcaster<int>;

            Assert.NotNull(b);

            b.Dispose();

            Assert.True(b.IsDisposed);

        }

    }
}
