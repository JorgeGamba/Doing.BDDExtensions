using System;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class FeatureSpecificationsSpecs : FeatureSpecifications
    {
        public class When_Given_and_When_are_both_implemented : FeatureSpecificationsSpecs
        {
            protected SimpleFeatureWithGivenAndWhen _result;

            public override void When() =>
                _result = new SimpleFeatureWithGivenAndWhen();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_first_the_Given_step() =>
                _result.OrderForGiven.ShouldBe(1);

            [Test]
            public void Should_use_last_the_When_step() =>
                _result.OrderForWhen.ShouldBe(2);
        }

        public class When_only_Given_is_implemented : FeatureSpecificationsSpecs
        {
            protected SimpleFeatureWithOnlyGiven _result;

            public override void When() =>
                _result = new SimpleFeatureWithOnlyGiven();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();
        }

        public class When_only_When_is_implemented : FeatureSpecificationsSpecs
        {
            protected SimpleFeatureWithOnlyWhen _result;

            public override void When() =>
                _result = new SimpleFeatureWithOnlyWhen();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();
        }

        public class When_the_When_is_at_root_and_Given_is_in_the_child : FeatureSpecificationsSpecs
        {
            protected RootWithOnlyWhenFeature.ChildWithGivenContext _result;

            public override void When() =>
                _result = new RootWithOnlyWhenFeature.ChildWithGivenContext();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_first_the_Given_step() =>
                _result.OrderForGiven.ShouldBe(1);

            [Test]
            public void Should_use_last_the_When_step() =>
                _result.OrderForWhen.ShouldBe(2);
        }

        public class When_the_When_is_at_root_and_Given_is_deep_nested : FeatureSpecificationsSpecs
        {
            protected RootWithOnlyWhenFeature.ChildWithoutStepsContext.DeepNestedContext _result;

            public override void When() =>
                _result = new RootWithOnlyWhenFeature.ChildWithoutStepsContext.DeepNestedContext();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_first_the_Given_step() =>
                _result.OrderForGiven.ShouldBe(1);

            [Test]
            public void Should_use_last_the_When_step() =>
                _result.OrderForWhen.ShouldBe(2);
        }

        public class When_the_When_is_at_root_and_Givens_are_in_two_levels : FeatureSpecificationsSpecs
        {
            protected RootWithOnlyWhenFeature.ChildWithGivenContext.NestedWithGivenContext _result;

            public override void When() =>
                _result = new RootWithOnlyWhenFeature.ChildWithGivenContext.NestedWithGivenContext();

            [Test]
            public void Should_use_the_higher_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_lower_Given_step() =>
                _result.NestedGivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_first_the_higher_Given_step() =>
                _result.OrderForGiven.ShouldBe(1);

            [Test]
            public void Should_use_second_the_lower_Given_step() =>
                _result.OrderForNestedGiven.ShouldBe(2);

            [Test]
            public void Should_use_last_the_When_step() =>
                _result.OrderForWhen.ShouldBe(3);
        }

        public class When_the_When_is_at_root_and_Givens_are_in_multiple_levels : FeatureSpecificationsSpecs
        {
            protected RootWithOnlyWhenFeature.ChildWithGivenContext.NestedWithGivenContext.GrandChildWithoutStepsContext.DeepestNestedContext _result;

            public override void When() =>
                _result = new RootWithOnlyWhenFeature.ChildWithGivenContext.NestedWithGivenContext.GrandChildWithoutStepsContext.DeepestNestedContext();

            [Test]
            public void Should_use_the_higher_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_middle_Given_step() =>
                _result.NestedGivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_lower_Given_step() =>
                _result.DeepestGivenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();

            [Test]
            public void Should_use_first_the_higher_Given_step() =>
                _result.OrderForGiven.ShouldBe(1);

            [Test]
            public void Should_use_second_the_middle_Given_step() =>
                _result.OrderForNestedGiven.ShouldBe(2);

            [Test]
            public void Should_use_third_the_deepest_Given_step() =>
                _result.OrderForDeepestGiven.ShouldBe(3);

            [Test]
            public void Should_use_last_the_When_step() =>
                _result.OrderForWhen.ShouldBe(4);
        }

        public class When_a_Given_throws : FeatureSpecificationsSpecs
        {
            protected Exception _exception;

            public override void When() =>
                _exception = Catch.Exception(() => new FeatureWithFailingGiven());

            [Test]
            public void Should_propagate_the_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_propagate_the_original_exception_type() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_propagate_the_original_message() =>
                _exception.Message.ShouldBe("sync given failed");
        }

        public class When_a_When_throws : FeatureSpecificationsSpecs
        {
            protected Exception _exception;

            public override void When() =>
                _exception = Catch.Exception(() => new FeatureWithFailingWhen());

            [Test]
            public void Should_propagate_the_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_propagate_the_original_exception_type() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_propagate_the_original_message() =>
                _exception.Message.ShouldBe("sync when failed");
        }
    }
}
