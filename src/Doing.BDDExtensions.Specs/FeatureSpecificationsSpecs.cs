using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    public class FeatureSpecificationsSpecs
    {
        public class When_the_Given_and_When_parts_are_both_implemented_by_a_simple_scenario_class
        {
            [OneTimeSetUp]
            public void Given() =>
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


            SimpleFeatureWithGivenAndWhen _result;
        }

        public class When_only_the_Given_part_is_implemented_by_a_simple_scenario_class
        {
            [OneTimeSetUp]
            public void Given() =>
                _result = new SimpleFeatureWithOnlyGiven();

            [Test]
            public void Should_use_the_Given_step() =>
                _result.GivenWasCalled.ShouldBeTrue();


            SimpleFeatureWithOnlyGiven _result;
        }

        public class When_only_the_When_part_is_implemented_by_a_simple_scenario_class
        {
            [OneTimeSetUp]
            public void When() =>
                _result = new SimpleFeatureWithOnlyWhen();

            [Test]
            public void Should_use_the_When_step() =>
                _result.WhenWasCalled.ShouldBeTrue();


            SimpleFeatureWithOnlyWhen _result;
        }

        public class When_the_When_is_in_the_root_level_and_the_Given_is_in_the_child_level
        {
            [OneTimeSetUp]
            public void Given() =>
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


            RootWithOnlyWhenFeature.ChildWithGivenContext _result;
        }

        public class When_the_When_is_in_the_root_level_and_the_Given_is_in_a_deep_nested_level
        {
            [OneTimeSetUp]
            public void Given() =>
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


            RootWithOnlyWhenFeature.ChildWithoutStepsContext.DeepNestedContext _result;
        }

        public class When_the_When_is_in_the_root_level_and_the_are_Givens_in_two_levels_below
        {
            [OneTimeSetUp]
            public void Given() =>
                _result = new RootWithOnlyWhenFeature.ChildWithGivenContext.NestedContext();

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


            RootWithOnlyWhenFeature.ChildWithGivenContext.NestedContext _result;
        }
    }
}