using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: Polymorphic returns (LoanApproval vs LoanRejection), 3-level hierarchy,
/// factory pattern specification, type-casting helpers for return types.
/// </summary>
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
            public void Should_produce_a_LoanApproval() =>
                _result.ShouldBeOfType<LoanApproval>();

            [Test]
            public void Should_set_the_loan_period_to_14_days() =>
                ResultAsApproval.LoanPeriodDays.ShouldBe(14);

            [Test]
            public void Should_calculate_the_due_date_correctly() =>
                ResultAsApproval.DueDate.ShouldBe(new DateTime(2024, 6, 15));

            [Test]
            public void Should_set_the_member_id() =>
                ResultAsApproval.MemberId.ShouldBe("M001");

            [Test]
            public void Should_set_the_book_id() =>
                ResultAsApproval.BookId.ShouldBe("978-0-13-468599-1");
        }

        public class And_the_member_is_Premium : When_the_member_is_eligible
        {
            public override void Given() =>
                _member = new Member("M002", "Bob", MembershipTier.Premium);

            [Test]
            public void Should_produce_a_LoanApproval() =>
                _result.ShouldBeOfType<LoanApproval>();

            [Test]
            public void Should_set_the_loan_period_to_21_days() =>
                ResultAsApproval.LoanPeriodDays.ShouldBe(21);
        }

        public class And_the_member_is_VIP : When_the_member_is_eligible
        {
            public override void Given() =>
                _member = new Member("M003", "Carol", MembershipTier.VIP);

            [Test]
            public void Should_produce_a_LoanApproval() =>
                _result.ShouldBeOfType<LoanApproval>();

            [Test]
            public void Should_set_the_loan_period_to_30_days() =>
                ResultAsApproval.LoanPeriodDays.ShouldBe(30);
        }
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
            public void Should_produce_a_LoanRejection() =>
                _result.ShouldBeOfType<LoanRejection>();

            [Test]
            public void Should_explain_the_rejection_reason() =>
                ResultAsRejection.Reason.ShouldContain("suspended");
        }

        public class Because_the_loan_limit_is_reached : When_the_member_is_not_eligible
        {
            public override void Given()
            {
                _member = new Member("M001", "Alice", MembershipTier.Standard);
                _activeLoans = 3;
            }

            [Test]
            public void Should_produce_a_LoanRejection() =>
                _result.ShouldBeOfType<LoanRejection>();

            [Test]
            public void Should_mention_the_loan_limit() =>
                ResultAsRejection.Reason.ShouldContain("loan limit");
        }
    }
}
