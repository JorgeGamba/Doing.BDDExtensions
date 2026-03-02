# Module-Level Orchestration with Mocks

> When to use: the SUT is a service/coordinator that depends on multiple collaborators, and you need to verify both return value and side effects

## Key Points
- Production class: `LendingService.Borrow(member, bookId, requestDate)` — coordinates book repository, loan records, fines, and loan request factory
- Base `Given()` creates all mocks, wires the service, and configures "happy path" stub returns
- Child `Given()` overrides a **single mock stub** — all other mocks remain in happy-path state
- Side-effect assertions use `Received()` / `DidNotReceive()` directly in `[Test]` methods
- Both return value and collaborator interactions are asserted in the same context class
- **Fractal chain**: `Should_approve_the_loan` traces to `When_the_member_is_eligible` at service level (LoanRequestFactorySpecs)

## Spec Code

```csharp
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
        var factory = new LoanRequestFactory(new LoanEligibilityEvaluator(maximumOutstandingFines: 10.00m));
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
        public void Should_approve_the_loan() => _result.ShouldBeOfType<LoanApproval>();

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
        public void Should_reject_the_loan() => _result.ShouldBeOfType<LoanRejection>();

        [Test]
        public void Should_not_update_the_book_state() =>
            _bookRepository.DidNotReceive().UpdateState(Arg.Any<string>(), Arg.Any<BookState>());

        [Test]
        public void Should_not_save_any_loan_record() =>
            _loanRecordRepository.DidNotReceive().Save(Arg.Any<LoanRecord>());
    }

    // When_the_outstanding_fines_exceed_the_limit and When_the_active_loans_are_at_the_tier_limit
    // follow the same mock-override pattern with single-stub Given() overrides
}
```
