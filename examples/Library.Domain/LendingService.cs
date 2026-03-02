namespace Library.Domain;

public class LendingService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRecordRepository _loanRecordRepository;
    private readonly IFineRepository _fineRepository;
    private readonly LoanRequestFactory _loanRequestFactory;

    public LendingService(
        IBookRepository bookRepository,
        ILoanRecordRepository loanRecordRepository,
        IFineRepository fineRepository,
        LoanRequestFactory loanRequestFactory)
    {
        _bookRepository = bookRepository;
        _loanRecordRepository = loanRecordRepository;
        _fineRepository = fineRepository;
        _loanRequestFactory = loanRequestFactory;
    }

    public ILoanRequest Borrow(Member member, string bookId, DateTime requestDate)
    {
        var bookState = _bookRepository.GetState(bookId);

        if (bookState != BookState.Available)
            return new LoanRejection(member.Id, bookId, $"Book is not available (current state: {bookState}).");

        var activeLoans = _loanRecordRepository.GetActiveLoansCount(member.Id);
        var outstandingFines = _fineRepository.GetOutstandingFines(member.Id);

        var request = _loanRequestFactory.Create(member, bookId, activeLoans, outstandingFines, requestDate);

        if (request is LoanApproval approval)
        {
            _bookRepository.UpdateState(bookId, BookState.OnLoan);
            _loanRecordRepository.Save(new LoanRecord(member.Id, bookId, requestDate, approval.DueDate));
        }

        return request;
    }
}
