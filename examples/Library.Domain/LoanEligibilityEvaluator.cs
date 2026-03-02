namespace Library.Domain;

public class LoanEligibilityEvaluator
{
    private readonly decimal _maximumOutstandingFines;

    public LoanEligibilityEvaluator(decimal maximumOutstandingFines)
    {
        _maximumOutstandingFines = maximumOutstandingFines;
    }

    public LoanEligibilityResult Evaluate(Member member, int activeLoans, Fine outstandingFines)
    {
        if (member == null) throw new ArgumentNullException(nameof(member));

        if (member.IsSuspended)
            return LoanEligibilityResult.Ineligible("Member account is suspended.");

        if (outstandingFines.Amount > _maximumOutstandingFines)
            return LoanEligibilityResult.Ineligible(
                $"Outstanding fines of {outstandingFines} exceed the maximum allowed ({_maximumOutstandingFines:C}).");

        var maxLoans = GetMaxLoansForTier(member.Tier);
        if (activeLoans >= maxLoans)
            return LoanEligibilityResult.Ineligible(
                $"Active loan limit of {maxLoans} reached for {member.Tier} membership.");

        return LoanEligibilityResult.Eligible(maxLoans - activeLoans);
    }

    private static int GetMaxLoansForTier(MembershipTier tier) => tier switch
    {
        MembershipTier.Standard => 3,
        MembershipTier.Premium => 7,
        MembershipTier.VIP => 15,
        _ => throw new ArgumentOutOfRangeException(nameof(tier))
    };
}

public class LoanEligibilityResult
{
    public bool IsEligible { get; }
    public string Reason { get; }
    public int RemainingSlots { get; }

    private LoanEligibilityResult(bool isEligible, string reason, int remainingSlots)
    {
        IsEligible = isEligible;
        Reason = reason;
        RemainingSlots = remainingSlots;
    }

    public static LoanEligibilityResult Eligible(int remainingSlots) =>
        new(true, string.Empty, remainingSlots);

    public static LoanEligibilityResult Ineligible(string reason) =>
        new(false, reason, 0);
}
