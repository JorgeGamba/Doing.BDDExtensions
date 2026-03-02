# Stateful Evaluator Pattern

> When to use: the subject under test is a stateful evaluator whose outcome changes based on one or more input conditions

## Key Points
- Production class: `LoanEligibilityEvaluator.Evaluate(member, activeLoans, outstandingFines)` — returns eligible/ineligible with reason based on suspension, fines, and tier limits
- Base `Given()` establishes a fully valid "happy path" — every field set to a passing value
- Root `When()` calls the evaluator once; result stored in `protected` field all children share
- Each child overrides `Given()` to deviate **one condition** from the happy path
- `And_` = complementary sub-condition; `But_` = contrasting sub-condition
- Fields that drive evaluation are `protected` so children can mutate them
- No mocks needed — evaluator is pure logic with no external dependencies
- **Fractal chain**: bottom of eligibility chain — `Should_be_eligible` is the outcome that LoanRequestFactory depends on

## Spec Code

```csharp
[TestFixture]
public class LoanEligibilityEvaluatorSpecs : FeatureSpecifications
{
    private readonly LoanEligibilityEvaluator _evaluator = new(maximumOutstandingFines: 10.00m);
    protected Member _member;
    protected int _activeLoans;
    protected Fine _outstandingFines;
    protected LoanEligibilityResult _result;

    public override void Given()
    {
        _member = new Member("M001", "Alice", MembershipTier.Standard);
        _activeLoans = 0;
        _outstandingFines = Fine.Zero;
    }

    public override void When() =>
        _result = _evaluator.Evaluate(_member, _activeLoans, _outstandingFines);

    public class When_the_member_is_in_good_standing : LoanEligibilityEvaluatorSpecs
    {
        [Test]
        public void Should_be_eligible() => _result.IsEligible.ShouldBeTrue();

        [Test]
        public void Should_indicate_the_remaining_slots() => _result.RemainingSlots.ShouldBe(3);
    }

    public class When_the_member_is_suspended : LoanEligibilityEvaluatorSpecs
    {
        public override void Given() => _member.IsSuspended = true;

        [Test]
        public void Should_be_ineligible() => _result.IsEligible.ShouldBeFalse();

        [Test]
        public void Should_explain_the_suspension() => _result.Reason.ShouldContain("suspended");
    }

    public class When_the_member_has_outstanding_fines : LoanEligibilityEvaluatorSpecs
    {
        public class And_the_fines_are_within_the_limit : When_the_member_has_outstanding_fines
        {
            public override void Given() => _outstandingFines = new Fine(9.99m);

            [Test]
            public void Should_still_be_eligible() => _result.IsEligible.ShouldBeTrue();
        }

        public class But_the_fines_exceed_the_limit : When_the_member_has_outstanding_fines
        {
            public override void Given() => _outstandingFines = new Fine(10.01m);

            [Test]
            public void Should_be_ineligible() => _result.IsEligible.ShouldBeFalse();
        }
    }

    // When_the_active_loans_are_at_the_tier_limit follows the same pattern
    // with And_/But_ children for Standard (3), Premium (7), VIP (5 of 15)
}
```
