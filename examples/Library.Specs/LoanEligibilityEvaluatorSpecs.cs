using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: Stateful evaluator, multiple condition paths, 3-level hierarchy,
/// progressive context accumulation, And_/But_ prefixes for condition variations.
/// </summary>
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
        public void Should_be_eligible() =>
            _result.IsEligible.ShouldBeTrue();

        [Test]
        public void Should_indicate_the_remaining_slots() =>
            _result.RemainingSlots.ShouldBe(3); // Standard tier = 3 max
    }

    public class When_the_member_is_suspended : LoanEligibilityEvaluatorSpecs
    {
        public override void Given() =>
            _member.IsSuspended = true;

        [Test]
        public void Should_be_ineligible() =>
            _result.IsEligible.ShouldBeFalse();

        [Test]
        public void Should_explain_the_suspension() =>
            _result.Reason.ShouldContain("suspended");
    }

    public class When_the_member_has_outstanding_fines : LoanEligibilityEvaluatorSpecs
    {
        public class And_the_fines_are_within_the_limit : When_the_member_has_outstanding_fines
        {
            public override void Given() =>
                _outstandingFines = new Fine(9.99m);

            [Test]
            public void Should_still_be_eligible() =>
                _result.IsEligible.ShouldBeTrue();
        }

        public class But_the_fines_exceed_the_limit : When_the_member_has_outstanding_fines
        {
            public override void Given() =>
                _outstandingFines = new Fine(10.01m);

            [Test]
            public void Should_be_ineligible() =>
                _result.IsEligible.ShouldBeFalse();

            [Test]
            public void Should_mention_the_outstanding_amount() =>
                _result.Reason.ShouldContain("Outstanding fines");
        }
    }

    public class When_the_member_has_reached_the_loan_limit : LoanEligibilityEvaluatorSpecs
    {
        public class And_the_member_is_Standard : When_the_member_has_reached_the_loan_limit
        {
            public override void Given()
            {
                _member = new Member("M001", "Alice", MembershipTier.Standard);
                _activeLoans = 3;
            }

            [Test]
            public void Should_be_ineligible() =>
                _result.IsEligible.ShouldBeFalse();

            [Test]
            public void Should_mention_the_limit_of_3() =>
                _result.Reason.ShouldContain("3");
        }

        public class And_the_member_is_Premium_with_7_active_loans : When_the_member_has_reached_the_loan_limit
        {
            public override void Given()
            {
                _member = new Member("M002", "Bob", MembershipTier.Premium);
                _activeLoans = 7;
            }

            [Test]
            public void Should_be_ineligible() =>
                _result.IsEligible.ShouldBeFalse();

            [Test]
            public void Should_mention_the_limit_of_7() =>
                _result.Reason.ShouldContain("7");
        }

        public class But_the_member_is_VIP_with_only_5_active_loans : When_the_member_has_reached_the_loan_limit
        {
            public override void Given()
            {
                _member = new Member("M003", "Carol", MembershipTier.VIP);
                _activeLoans = 5;
            }

            [Test]
            public void Should_still_be_eligible() =>
                _result.IsEligible.ShouldBeTrue();

            [Test]
            public void Should_have_10_remaining_slots() =>
                _result.RemainingSlots.ShouldBe(10); // VIP max 15 - 5 active = 10
        }
    }
}
