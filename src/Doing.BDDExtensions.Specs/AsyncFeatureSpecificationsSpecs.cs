using System;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class AsyncFeatureSpecificationsSpecs : FeatureSpecifications
    {
        public class When_async_Given_and_When_are_both_implemented : AsyncFeatureSpecificationsSpecs
        {
            protected AsyncFeatureWithGivenAndWhen _result;

            public override void When() =>
                _result = new AsyncFeatureWithGivenAndWhen();

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

        public class When_only_async_Given_is_implemented : AsyncFeatureSpecificationsSpecs
        {
            protected AsyncFeatureWithOnlyAsyncGiven _result;

            public override void When() =>
                _result = new AsyncFeatureWithOnlyAsyncGiven();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();
        }

        public class When_only_async_When_is_implemented : AsyncFeatureSpecificationsSpecs
        {
            protected AsyncFeatureWithOnlyAsyncWhen _result;

            public override void When() =>
                _result = new AsyncFeatureWithOnlyAsyncWhen();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();
        }

        public class When_async_When_is_at_root_and_async_Given_is_in_child : AsyncFeatureSpecificationsSpecs
        {
            protected AsyncRootWithWhenFeature.ChildWithAsyncGivenContext _result;

            public override void When() =>
                _result = new AsyncRootWithWhenFeature.ChildWithAsyncGivenContext();

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

        public class When_async_When_is_at_root_and_async_Givens_are_in_two_levels : AsyncFeatureSpecificationsSpecs
        {
            protected AsyncRootWithWhenFeature.ChildWithAsyncGivenContext.NestedWithAsyncGivenContext _result;

            public override void When() =>
                _result = new AsyncRootWithWhenFeature.ChildWithAsyncGivenContext.NestedWithAsyncGivenContext();

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

        public class When_an_async_Given_throws : AsyncFeatureSpecificationsSpecs
        {
            protected Exception _exception;

            public override void When() =>
                _exception = Catch.Exception(() => new AsyncFeatureWithFailingGiven());

            [Test]
            public void Should_propagate_the_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_propagate_the_original_exception_type() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_propagate_the_original_message() =>
                _exception.Message.ShouldBe("async given failed");
        }

        public class When_an_async_When_throws : AsyncFeatureSpecificationsSpecs
        {
            protected Exception _exception;

            public override void When() =>
                _exception = Catch.Exception(() => new AsyncFeatureWithFailingWhen());

            [Test]
            public void Should_propagate_the_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_propagate_the_original_exception_type() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_propagate_the_original_message() =>
                _exception.Message.ShouldBe("async when failed");
        }
    }
}
