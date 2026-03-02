namespace Library.Domain;

public class LoanRequestFactory
{
    private readonly LoanEligibilityEvaluator _eligibilityEvaluator;

    public LoanRequestFactory(LoanEligibilityEvaluator eligibilityEvaluator)
    {
        _eligibilityEvaluator = eligibilityEvaluator;
    }

    public ILoanRequest Create(Member member, string bookId, int activeLoans, Fine outstandingFines, DateTime requestDate)
    {
        var eligibility = _eligibilityEvaluator.Evaluate(member, activeLoans, outstandingFines);

        if (!eligibility.IsEligible)
            return new LoanRejection(member.Id, bookId, eligibility.Reason);

        var loanPeriod = GetLoanPeriodForTier(member.Tier);
        var dueDate = requestDate.AddDays(loanPeriod);

        return new LoanApproval(member.Id, bookId, dueDate, loanPeriod);
    }

    private static int GetLoanPeriodForTier(MembershipTier tier) => tier switch
    {
        MembershipTier.Standard => 14,
        MembershipTier.Premium => 21,
        MembershipTier.VIP => 30,
        _ => throw new ArgumentOutOfRangeException(nameof(tier))
    };
}
