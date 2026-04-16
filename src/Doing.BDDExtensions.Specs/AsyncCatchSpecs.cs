using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    public class AsyncCatchSpecs
    {
        public class When_the_async_action_does_not_throw_an_exception
        {
            public When_the_async_action_does_not_throw_an_exception() =>
                _exception = Catch.Exception(async () => await Task.Yield());

            [Test]
            public void Should_not_throw_an_exception() =>
                _exception.ShouldBeNull();


            Exception _exception;
        }

        public class When_the_async_action_throws_an_exception
        {
            public When_the_async_action_throws_an_exception() =>
                _exception = Catch.Exception(async () =>
                {
                    await Task.Yield();
                    throw _formerException;
                });

            [Test]
            public void Should_throw_an_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_throw_exactly_the_former_exception() =>
                _exception.ShouldBe(_formerException);

            [Test]
            public void Should_not_wrap_in_an_AggregateException() =>
                _exception.ShouldBeOfType<InvalidOperationException>();


            Exception _exception;
            Exception _formerException = new InvalidOperationException("async error");
        }
    }
}
