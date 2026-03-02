# State Machine Specification Pattern

> When to use: subject has explicit states and transition methods that enforce valid-from-state rules

## Key Points
- Production class: `BookLifecycle` — state machine with `Lend()`, `Return()`, `ReportLost()` transitions enforcing valid-from-state rules
- One `When_` class per transition method — never combine transitions
- Always wrap the transition in `Catch.Exception(...)` at `When()` level, even for happy path
- `And_` for states that make transition valid; `But_` for states that make it invalid
- Assert `_exception.ShouldBeNull()` on happy-path to make no-throw explicit
- Assert `_book.CurrentState` only on happy-path — state is undefined after throw
- Message assertions (`ShouldContain`) document what the exception surfaces

## Spec Code

```csharp
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
            public override void Given() => _book = new BookLifecycle(BookState.Available);

            [Test]
            public void Should_transition_to_OnLoan() => _book.CurrentState.ShouldBe(BookState.OnLoan);

            [Test]
            public void Should_not_throw() => _exception.ShouldBeNull();
        }

        public class But_the_book_is_already_OnLoan : When_lending_a_book
        {
            public override void Given() => _book = new BookLifecycle(BookState.OnLoan);

            [Test]
            public void Should_throw_an_InvalidOperationException() =>
                _exception.ShouldBeOfType<InvalidOperationException>();

            [Test]
            public void Should_mention_the_required_state() =>
                _exception.Message.ShouldContain("Available");
        }

        public class But_the_book_is_Reserved : When_lending_a_book
        {
            public override void Given() => _book = new BookLifecycle(BookState.Reserved);

            [Test]
            public void Should_throw_because_Lend_requires_Available() =>
                _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }

    // When_returning_a_book (And_OnLoan, And_Overdue valid; But_Available invalid)
    // and When_reporting_a_book_as_lost (And_OnLoan, And_Overdue valid; But_Available, But_Lost invalid)
    // follow the same valid/invalid state pattern per transition
}
```
