using Doing.BDDExtensions;
using Library.Domain;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: Module-level orchestration, NSubstitute mocks, 3-level hierarchy,
/// verifying collaborator interactions, acceptance/integration style specification.
/// </summary>
[TestFixture]
public class LendingServiceSpecs : FeatureSpecifications
{
    protected IBookRepository _bookRepository;
    protected ILoanRecordRepository _loanRecordRepository;
    protected IFineRepository _fineRepository;
    protected LendingService _service;

    protected Member _member;
    protected string _bookId;
    protected DateTime _requestDate;
    protected ILoanRequest _result;

    public override void Given()
    {
        _bookRepository = Substitute.For<IBookRepository>();
        _loanRecordRepository = Substitute.For<ILoanRecordRepository>();
        _fineRepository = Substitute.For<IFineRepository>();

        var eligibilityEvaluator = new LoanEligibilityEvaluator(maximumOutstandingFines: 10.00m);
        var factory = new LoanRequestFactory(eligibilityEvaluator);
        _service = new LendingService(_bookRepository, _loanRecordRepository, _fineRepository, factory);

        _member = new Member("M001", "Alice", MembershipTier.Standard);
        _bookId = "B001";
        _requestDate = new DateTime(2024, 6, 1);

        _bookRepository.GetState(_bookId).Returns(BookState.Available);
        _loanRecordRepository.GetActiveLoansCount(_member.Id).Returns(0);
        _fineRepository.GetOutstandingFines(_member.Id).Returns(Fine.Zero);
    }

    public override void When() =>
        _result = _service.Borrow(_member, _bookId, _requestDate);

    public class When_the_book_is_available_and_member_is_eligible : LendingServiceSpecs
    {
        [Test]
        public void Should_approve_the_loan() =>
            _result.ShouldBeOfType<LoanApproval>();

        [Test]
        public void Should_update_the_book_state_to_OnLoan() =>
            _bookRepository.Received().UpdateState(_bookId, BookState.OnLoan);

        [Test]
        public void Should_save_a_loan_record() =>
            _loanRecordRepository.Received().Save(
                Arg.Is<LoanRecord>(r => r.MemberId == "M001" && r.BookId == "B001"));
    }

    public class When_the_book_is_not_available : LendingServiceSpecs
    {
        public override void Given() =>
            _bookRepository.GetState(_bookId).Returns(BookState.OnLoan);

        [Test]
        public void Should_reject_the_loan() =>
            _result.ShouldBeOfType<LoanRejection>();

        [Test]
        public void Should_explain_the_book_is_unavailable() =>
            ((LoanRejection)_result).Reason.ShouldContain("not available");

        [Test]
        public void Should_not_update_the_book_state() =>
            _bookRepository.DidNotReceive().UpdateState(Arg.Any<string>(), Arg.Any<BookState>());

        [Test]
        public void Should_not_save_any_loan_record() =>
            _loanRecordRepository.DidNotReceive().Save(Arg.Any<LoanRecord>());
    }

    public class When_the_outstanding_fines_exceed_the_limit : LendingServiceSpecs
    {
        public override void Given() =>
            _fineRepository.GetOutstandingFines(_member.Id).Returns(new Fine(15.00m));

        [Test]
        public void Should_reject_the_loan() =>
            _result.ShouldBeOfType<LoanRejection>();

        [Test]
        public void Should_not_update_the_book_state() =>
            _bookRepository.DidNotReceive().UpdateState(Arg.Any<string>(), Arg.Any<BookState>());
    }

    public class When_the_active_loans_are_at_the_tier_limit : LendingServiceSpecs
    {
        public override void Given() =>
            _loanRecordRepository.GetActiveLoansCount(_member.Id).Returns(3);

        [Test]
        public void Should_reject_the_loan() =>
            _result.ShouldBeOfType<LoanRejection>();

        [Test]
        public void Should_mention_the_loan_limit_in_the_reason() =>
            ((LoanRejection)_result).Reason.ShouldContain("loan limit");
    }
}
