namespace Doing.BDDExtensions.Specs
{
    public class SimpleFeatureWithGivenAndWhen : FeatureSpecifications
    {
        private int _order;

        public bool GivenWasCalled { get; set; }

        public bool WhenWasCalled { get; set; }

        public int OrderForGiven { get; set; }

        public int OrderForWhen { get; set; }

        public override void Given()
        {
            GivenWasCalled = true;
            OrderForGiven = ++_order;
        }

        public override void When()
        {
            WhenWasCalled = true;
            OrderForWhen = ++_order;
        }
    }

    public class SimpleFeatureWithOnlyGiven : FeatureSpecifications
    {
        public bool GivenWasCalled { get; set; }

        public override void Given() => GivenWasCalled = true;
    }

    public class SimpleFeatureWithOnlyWhen : FeatureSpecifications
    {
        public bool WhenWasCalled { get; set; }

        public override void When() => WhenWasCalled = true;
    }

    public class RootWithOnlyWhenFeature : FeatureSpecifications
    {
        private int _order;

        public bool GivenWasCalled { get; set; }

        public bool WhenWasCalled { get; set; }

        public int OrderForGiven { get; set; }

        public int OrderForWhen { get; set; }

        public override void When()
        {
            WhenWasCalled = true;
            OrderForWhen = ++_order;
        }

        public class ChildWithGivenContext : RootWithOnlyWhenFeature
        {
            public override void Given()
            {
                GivenWasCalled = true;
                OrderForGiven = ++_order;
            }

            public class NestedContext : ChildWithGivenContext
            {
                public bool NestedGivenWasCalled { get; set; }

                public int OrderForNestedGiven { get; set; }

                public override void Given()
                {
                    NestedGivenWasCalled = true;
                    OrderForNestedGiven = ++_order;
                }
            }
        }

        public class ChildWithoutStepsContext : RootWithOnlyWhenFeature
        {
            public class DeepNestedContext : ChildWithoutStepsContext
            {
                public override void Given()
                {
                    GivenWasCalled = true;
                    OrderForGiven = ++_order;
                }
            }
        }
    }
}