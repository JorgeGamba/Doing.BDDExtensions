using System;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    public class CatchSpecs
    {
        public class When_does_not_happen_an_exception
        {
            public When_does_not_happen_an_exception() => 
                _exception = Catch.Exception(DoSomeAction);

            [Test]
            public void Should_not_throw_an_exception() =>
                _exception.ShouldBeNull();

            [Test]
            public void Should_execute_the_action() =>
                _actionDone.ShouldBeTrue();

            void DoSomeAction() => _actionDone = true;


            Exception _exception;
            bool _actionDone;
        }

        public class When_does_happen_an_exception
        {
            public When_does_happen_an_exception() => 
                _exception = Catch.Exception(DoSomeTroubleAction);

            [Test]
            public void Should_throw_an_exception() =>
                _exception.ShouldNotBeNull();

            [Test]
            public void Should_throw_exactly_the_former_exception() =>
                _exception.ShouldBe(_formerException);

            void DoSomeTroubleAction() => throw _formerException;


            Exception _exception;
            bool _actionDone;
            Exception _formerException = new Exception("Some exception");
        }
    }
}