using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class AsyncCatchSpecs : FeatureSpecifications
    {
        protected Func<Task> _action;
        protected Exception _exception;

        public override void When() =>
            _exception = Catch.Exception(_action);

        public class When_the_async_action_does_not_throw : AsyncCatchSpecs
        {
            public override void Given() =>
                _action = async () => await Task.Yield();

            [Test]
            public void Should_not_throw_an_exception() =>
                _exception.ShouldBeNull();
        }

        public class When_the_async_action_throws : AsyncCatchSpecs
        {
            protected Exception _formerException = new InvalidOperationException("async error");

            public override void Given() =>
                _action = async () =>
                {
                    await Task.Yield();
                    throw _formerException;
                };

            [Test]
            public void Should_throw_an_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_throw_exactly_the_former_exception() =>
                _exception.ShouldBe(_formerException);

            [Test]
            public void Should_not_wrap_in_an_AggregateException() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }
}
