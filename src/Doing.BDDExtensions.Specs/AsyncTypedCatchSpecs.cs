using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class AsyncTypedCatchSpecs : FeatureSpecifications
    {
        protected Func<Task> _action;
        protected InvalidOperationException _result;

        public override void When() =>
            _result = Catch.Exception<InvalidOperationException>(_action);

        public class When_the_async_action_throws_the_expected_exception_type : AsyncTypedCatchSpecs
        {
            public override void Given() =>
                _action = async () =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("async expected");
                };

            [Test]
            public void Should_return_the_typed_exception() =>
                _result.ShouldNotBeNull();

            [Test]
            public void Should_preserve_the_message() =>
                _result.Message.ShouldBe("async expected");
        }

        public class When_the_async_action_throws_a_different_exception_type : AsyncTypedCatchSpecs
        {
            public override void Given() =>
                _action = async () =>
                {
                    await Task.Yield();
                    throw new ArgumentException("wrong type");
                };

            [Test]
            public void Should_return_null() =>
                _result.ShouldBeNull();
        }
    }
}
