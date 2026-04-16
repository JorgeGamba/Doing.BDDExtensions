using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class TypedCatchSpecs : FeatureSpecifications
    {
        protected Action _action;
        protected InvalidOperationException _result;

        public override void When() =>
            _result = Catch.Exception<InvalidOperationException>(_action);

        public class When_the_action_throws_the_expected_exception_type : TypedCatchSpecs
        {
            public override void Given() =>
                _action = () => throw new InvalidOperationException("expected");

            [Test]
            public void Should_return_the_typed_exception() =>
                _result.ShouldNotBeNull();

            [Test]
            public void Should_preserve_the_message() =>
                _result.Message.ShouldBe("expected");
        }

        public class When_the_action_throws_a_different_exception_type : TypedCatchSpecs
        {
            public override void Given() =>
                _action = () => throw new ArgumentException("wrong type");

            [Test]
            public void Should_return_null() =>
                _result.ShouldBeNull();
        }

        public class When_the_action_does_not_throw : TypedCatchSpecs
        {
            public override void Given() =>
                _action = () => { };

            [Test]
            public void Should_return_null() =>
                _result.ShouldBeNull();
        }
    }
}
