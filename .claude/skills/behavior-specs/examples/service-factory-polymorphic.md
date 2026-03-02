# Factory with Polymorphic Return Type

> When to use: the SUT is a factory that returns one of several concrete types behind a shared interface, and the type itself is part of the expected behavior

## Key Points
- Production class: `LoanRequestFactory.Create(member, bookId, activeLoans, outstandingFines, requestDate)` — returns `LoanApproval` or `LoanRejection` based on eligibility
- Base `Given()` initializes factory + inputs; `_result` typed as `ILoanRequest` (the interface)
- Helper cast properties (`ResultAsApproval`, `ResultAsRejection`) on base avoid repetitive casts
- First assertion: `_result.ShouldBeOfType<LoanApproval>()` — type before properties
- `Because_` prefix names rejection reasons at leaf level
- **Fractal chain**: `Should_produce_a_LoanApproval` traces to `When_the_member_is_in_good_standing` at evaluator level (LoanEligibilityEvaluatorSpecs)

## Spec Code

```csharp
[TestFixture]
public class LoanRequestFactorySpecs : FeatureSpecifications
{
    private readonly LoanEligibilityEvaluator _eligibilityEvaluator = new(maximumOutstandingFines: 10.00m);
    private LoanRequestFactory _factory;
    protected Member _member;
    protected string _bookId;
    protected int _activeLoans;
    protected Fine _outstandingFines;
    protected DateTime _requestDate;
    protected ILoanRequest _result;

    public override void Given()
    {
        _factory = new LoanRequestFactory(_eligibilityEvaluator);
        _bookId = "978-0-13-468599-1";
        _activeLoans = 0;
        _outstandingFines = Fine.Zero;
        _requestDate = new DateTime(2024, 6, 1);
    }

    public override void When() =>
        _result = _factory.Create(_member, _bookId, _activeLoans, _outstandingFines, _requestDate);

    protected LoanApproval ResultAsApproval => (LoanApproval)_result;
    protected LoanRejection ResultAsRejection => (LoanRejection)_result;

    public class When_the_member_is_eligible : LoanRequestFactorySpecs
    {
        public class And_the_member_is_Standard : When_the_member_is_eligible
        {
            public override void Given() =>
                _member = new Member("M001", "Alice", MembershipTier.Standard);

            [Test]
            public void Should_produce_a_LoanApproval() => _result.ShouldBeOfType<LoanApproval>();

            [Test]
            public void Should_set_the_loan_period_to_14_days() => ResultAsApproval.LoanPeriodDays.ShouldBe(14);
        }

        // And_the_member_is_Premium (21 days), And_the_member_is_VIP (30 days) follow the same pattern
    }

    public class When_the_member_is_not_eligible : LoanRequestFactorySpecs
    {
        public class Because_the_member_is_suspended : When_the_member_is_not_eligible
        {
            public override void Given()
            {
                _member = new Member("M001", "Alice", MembershipTier.Standard);
                _member.IsSuspended = true;
            }

            [Test]
            public void Should_produce_a_LoanRejection() => _result.ShouldBeOfType<LoanRejection>();

            [Test]
            public void Should_explain_the_rejection_reason() => ResultAsRejection.Reason.ShouldContain("suspended");
        }

        // Because_the_loan_limit_is_reached follows the same pattern
    }
}
```
