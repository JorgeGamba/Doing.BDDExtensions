using System;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class CatchSpecs : FeatureSpecifications
    {
        protected Action _action;
        protected Exception _exception;

        public override void When() =>
            _exception = Catch.Exception(_action);

        public class When_the_action_does_not_throw : CatchSpecs
        {
            protected bool _actionDone;

            public override void Given() =>
                _action = () => _actionDone = true;

            [Test]
            public void Should_not_throw_an_exception() =>
                _exception.ShouldBeNull();

            [Test]
            public void Should_execute_the_action() =>
                _actionDone.ShouldBeTrue();
        }

        public class When_the_action_throws : CatchSpecs
        {
            protected Exception _formerException = new Exception("Some exception");

            public override void Given() =>
                _action = () => throw _formerException;

            [Test]
            public void Should_throw_an_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_throw_exactly_the_former_exception() =>
                _exception.ShouldBe(_formerException);
        }
    }
}
