using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: State machine transitions, invalid transition exceptions,
/// 3-4 level hierarchy, Catch.Exception for invalid states.
/// </summary>
[TestFixture]
public class BookLifecycleSpecs : FeatureSpecifications
{
    protected BookLifecycle _book;
    protected Exception _exception;

    public class When_lending_a_book : BookLifecycleSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _book.Lend());

        public class And_the_book_is_Available : When_lending_a_book
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Available);

            [Test]
            public void Should_transition_to_OnLoan() =>
                _book.CurrentState.ShouldBe(BookState.OnLoan);

            [Test]
            public void Should_not_throw() =>
                _exception.ShouldBeNull();
        }

        public class But_the_book_is_already_OnLoan : When_lending_a_book
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.OnLoan);

            [Test]
            public void Should_throw_an_InvalidOperationException() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_mention_the_required_state() =>
                _exception.Message.ShouldContain("Available");
        }

        public class But_the_book_is_Reserved : When_lending_a_book
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Reserved);

            [Test]
            public void Should_throw_because_Lend_requires_Available() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }

    public class When_returning_a_book : BookLifecycleSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _book.Return());

        public class And_the_book_is_OnLoan : When_returning_a_book
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.OnLoan);

            [Test]
            public void Should_transition_to_Available() =>
                _book.CurrentState.ShouldBe(BookState.Available);
        }

        public class And_the_book_is_Overdue : When_returning_a_book
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Overdue);

            [Test]
            public void Should_also_transition_to_Available() =>
                _book.CurrentState.ShouldBe(BookState.Available);
        }

        public class But_the_book_is_Available : When_returning_a_book
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Available);

            [Test]
            public void Should_throw_an_InvalidOperationException() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_mention_OnLoan_or_Overdue_as_valid_states() =>
                _exception.Message.ShouldContain("Must be OnLoan or Overdue");
        }
    }

    public class When_marking_a_book_as_overdue : BookLifecycleSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _book.MarkOverdue());

        public class And_the_book_is_OnLoan : When_marking_a_book_as_overdue
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.OnLoan);

            [Test]
            public void Should_transition_to_Overdue() =>
                _book.CurrentState.ShouldBe(BookState.Overdue);
        }

        public class But_the_book_is_Available : When_marking_a_book_as_overdue
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Available);

            [Test]
            public void Should_throw_because_only_OnLoan_books_can_become_Overdue() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }

    public class When_reporting_a_book_as_lost : BookLifecycleSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _book.ReportLost());

        public class And_the_book_is_OnLoan : When_reporting_a_book_as_lost
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.OnLoan);

            [Test]
            public void Should_transition_to_Lost() =>
                _book.CurrentState.ShouldBe(BookState.Lost);
        }

        public class And_the_book_is_Overdue : When_reporting_a_book_as_lost
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Overdue);

            [Test]
            public void Should_transition_to_Lost() =>
                _book.CurrentState.ShouldBe(BookState.Lost);
        }

        public class But_the_book_is_Available : When_reporting_a_book_as_lost
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Available);

            [Test]
            public void Should_throw_because_Available_books_cannot_be_lost() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }

        public class But_the_book_is_already_Lost : When_reporting_a_book_as_lost
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Lost);

            [Test]
            public void Should_throw_because_it_is_already_lost() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }

    public class When_fulfilling_a_reservation : BookLifecycleSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _book.FulfillReservation());

        public class And_the_book_is_Reserved : When_fulfilling_a_reservation
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Reserved);

            [Test]
            public void Should_transition_to_OnLoan() =>
                _book.CurrentState.ShouldBe(BookState.OnLoan);
        }

        public class But_the_book_is_Available : When_fulfilling_a_reservation
        {
            public override void Given() =>
                _book = new BookLifecycle(BookState.Available);

            [Test]
            public void Should_throw_because_it_must_be_Reserved_first() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }
}
