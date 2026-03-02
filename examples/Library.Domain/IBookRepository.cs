namespace Library.Domain;

public interface IBookRepository
{
    BookState GetState(string bookId);
    void UpdateState(string bookId, BookState newState);
}

public interface ILoanRecordRepository
{
    int GetActiveLoansCount(string memberId);
    void Save(LoanRecord record);
}

public interface IFineRepository
{
    Fine GetOutstandingFines(string memberId);
}

public record LoanRecord(string MemberId, string BookId, DateTime BorrowDate, DateTime DueDate);
