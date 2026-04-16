using System;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    public class AsyncFeatureSpecificationsSpecs
    {
        public class When_the_async_Given_and_When_parts_are_both_implemented
        {
            [OneTimeSetUp]
            public void Given() =>
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


            AsyncFeatureWithGivenAndWhen _result;
        }

        public class When_only_the_async_Given_part_is_implemented
        {
            [OneTimeSetUp]
            public void Given() =>
                _result = new AsyncFeatureWithOnlyAsyncGiven();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();


            AsyncFeatureWithOnlyAsyncGiven _result;
        }

        public class When_only_the_async_When_part_is_implemented
        {
            [OneTimeSetUp]
            public void When() =>
                _result = new AsyncFeatureWithOnlyAsyncWhen();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();


            AsyncFeatureWithOnlyAsyncWhen _result;
        }

        public class When_the_async_When_is_in_the_root_and_the_async_Given_is_in_the_child_level
        {
            [OneTimeSetUp]
            public void Given() =>
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


            AsyncRootWithWhenFeature.ChildWithAsyncGivenContext _result;
        }

        public class When_the_async_When_is_in_the_root_and_async_Givens_are_in_two_levels_below
        {
            [OneTimeSetUp]
            public void Given() =>
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


            AsyncRootWithWhenFeature.ChildWithAsyncGivenContext.NestedWithAsyncGivenContext _result;
        }

        public class When_an_async_Given_throws_an_exception
        {
            [OneTimeSetUp]
            public void Given() =>
                _exception = Catch.Exception(() => new AsyncFeatureWithFailingGiven());

            [Test]
            public void Should_propagate_the_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_propagate_the_original_exception_type() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_propagate_the_original_exception_message() =>
                _exception.Message.ShouldBe("async given failed");


            Exception _exception;
        }

        public class When_an_async_When_throws_an_exception
        {
            [OneTimeSetUp]
            public void Given() =>
                _exception = Catch.Exception(() => new AsyncFeatureWithFailingWhen());

            [Test]
            public void Should_propagate_the_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_propagate_the_original_exception_type() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_propagate_the_original_exception_message() =>
                _exception.Message.ShouldBe("async when failed");


            Exception _exception;
        }
    }
}
