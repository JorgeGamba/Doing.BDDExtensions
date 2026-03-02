# Fractal Spec Chain Across Pyramid Levels

> When to use: building specs for a feature that spans multiple layers — each level's outcome becomes the next level's precondition

## Key Points
- Domain: Library lending — Application (API) → Module (LendingService) → Service (LoanRequestFactory)
- **THEN@N = GIVEN@N+1**: `Should_produce_a_loan` at application → `When_a_loan_is_being_produced` at module
- **Naming correspondence**: `Should_` at level N is traceable to `When_` at level N+1 — same domain concept, different perspective
- **Outside-in order**: write application first, then module, then service — each `Should_` reveals what the next level must specify
- **Deferred scenarios**: use `[Ignore("Deferred: reason")]` for combinatorial branches not yet needed — visible in test runner as "Skipped", compilable, refactoring-safe

## Spec Code — Three Connected Levels

```csharp
// ── Level 1: Application (LibraryAPISpecs.cs) ─────────────────────────
// THEN here ↓ becomes GIVEN at Level 2

public class When_borrowing_a_book : LibraryAPISpecs
{
    public override void When() => _response = _server.Post("/books/B001/borrow");

    public class And_the_book_is_available : When_borrowing_a_book
    {
        [Test]
        public void Should_return_200_OK() => ((int)_response.StatusCode).ShouldBe(200);

        [Test]
        public void Should_produce_a_loan_with_correct_due_date()  // ← THEN: traces to Module
        {
            var body = _response.ReadJson();
            body["dueDate"].ShouldNotBeNull();
        }
    }
}

// ── Level 2: Module (LendingServiceSpecs.cs) ─────────────────────────
// When_ mirrors application's Should_ ↑ ; THEN here ↓ becomes GIVEN at Level 3

public class When_a_loan_is_being_produced : LendingServiceSpecs  // ← mirrors application THEN
{
    public override void Given() =>
        _bookRepository.GetState(_bookId).Returns(BookState.Available);

    [Test]
    public void Should_approve_the_loan() => _result.ShouldBeOfType<LoanApproval>();

    [Test]
    public void Should_delegate_to_factory_with_member_tier()  // ← THEN: traces to Service
    {
        var approval = (LoanApproval)_result;
        approval.LoanPeriodDays.ShouldBeGreaterThan(0);
    }

    [TestFixture, Ignore("Deferred: Premium tier loan period — exercise when Premium pricing confirmed")]
    public class And_the_member_is_Premium : When_a_loan_is_being_produced { }
}

// ── Level 3: Service (LoanRequestFactorySpecs.cs) ────────────────────
// When_ mirrors module's Should_ ↑

public class When_the_member_is_eligible : LoanRequestFactorySpecs  // ← mirrors module THEN
{
    public class And_the_member_is_Standard : When_the_member_is_eligible
    {
        public override void Given() =>
            _member = new Member("M001", "Alice", MembershipTier.Standard);

        [Test]
        public void Should_produce_a_LoanApproval() =>
            _result.ShouldBeOfType<LoanApproval>();

        [Test]
        public void Should_set_the_loan_period_to_14_days() =>  // ← exhaustive at unit/service level
            ResultAsApproval.LoanPeriodDays.ShouldBe(14);
    }

    [TestFixture, Ignore("Deferred: VIP 30-day loan period — same pattern as Standard")]
    public class And_the_member_is_VIP : When_the_member_is_eligible { }
}
```
