namespace Library.Domain;

public interface ILoanRequest
{
    string MemberId { get; }
    string BookId { get; }
}

public class LoanApproval : ILoanRequest
{
    public string MemberId { get; }
    public string BookId { get; }
    public DateTime DueDate { get; }
    public int LoanPeriodDays { get; }

    public LoanApproval(string memberId, string bookId, DateTime dueDate, int loanPeriodDays)
    {
        MemberId = memberId;
        BookId = bookId;
        DueDate = dueDate;
        LoanPeriodDays = loanPeriodDays;
    }
}

public class LoanRejection : ILoanRequest
{
    public string MemberId { get; }
    public string BookId { get; }
    public string Reason { get; }

    public LoanRejection(string memberId, string bookId, string reason)
    {
        MemberId = memberId;
        BookId = bookId;
        Reason = reason;
    }
}
