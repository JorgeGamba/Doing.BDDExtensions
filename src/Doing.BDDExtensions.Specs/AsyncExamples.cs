using System.Threading.Tasks;

namespace Doing.BDDExtensions.Specs
{
    public class AsyncFeatureWithGivenAndWhen : FeatureSpecifications
    {
        private int _order;

        public bool GivenWasCalled { get; set; }

        public bool WhenWasCalled { get; set; }

        public int OrderForGiven { get; set; }

        public int OrderForWhen { get; set; }

        public override async void Given()
        {
            await Task.Yield();
            GivenWasCalled = true;
            OrderForGiven = ++_order;
        }

        public override async void When()
        {
            await Task.Yield();
            WhenWasCalled = true;
            OrderForWhen = ++_order;
        }
    }

    public class AsyncFeatureWithOnlyAsyncGiven : FeatureSpecifications
    {
        public bool GivenWasCalled { get; set; }

        public override async void Given()
        {
            await Task.Yield();
            GivenWasCalled = true;
        }
    }

    public class AsyncFeatureWithOnlyAsyncWhen : FeatureSpecifications
    {
        public bool WhenWasCalled { get; set; }

        public override async void When()
        {
            await Task.Yield();
            WhenWasCalled = true;
        }
    }

    public class AsyncRootWithWhenFeature : FeatureSpecifications
    {
        private int _order;

        public bool GivenWasCalled { get; set; }

        public bool WhenWasCalled { get; set; }

        public int OrderForGiven { get; set; }

        public int OrderForWhen { get; set; }

        public override async void When()
        {
            await Task.Yield();
            WhenWasCalled = true;
            OrderForWhen = ++_order;
        }

        public class ChildWithAsyncGivenContext : AsyncRootWithWhenFeature
        {
            public override async void Given()
            {
                await Task.Yield();
                GivenWasCalled = true;
                OrderForGiven = ++_order;
            }

            public class NestedWithAsyncGivenContext : ChildWithAsyncGivenContext
            {
                public bool NestedGivenWasCalled { get; set; }

                public int OrderForNestedGiven { get; set; }

                public override async void Given()
                {
                    await Task.Yield();
                    NestedGivenWasCalled = true;
                    OrderForNestedGiven = ++_order;
                }
            }
        }
    }

    public class AsyncFeatureWithFailingGiven : FeatureSpecifications
    {
        public override async void Given()
        {
            await Task.Yield();
            throw new System.InvalidOperationException("async given failed");
        }
    }

    public class AsyncFeatureWithFailingWhen : FeatureSpecifications
    {
        public override async void Given()
        {
            await Task.Yield();
        }

        public override async void When()
        {
            await Task.Yield();
            throw new System.InvalidOperationException("async when failed");
        }
    }
}
