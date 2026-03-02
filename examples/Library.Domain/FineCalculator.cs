namespace Library.Domain;

public class FineCalculator
{
    private readonly decimal _baseRatePerDay;
    private readonly decimal _maximumFine;

    public FineCalculator(decimal baseRatePerDay, decimal maximumFine)
    {
        if (baseRatePerDay <= 0)
            throw new ArgumentException("Base rate must be positive.", nameof(baseRatePerDay));
        if (maximumFine <= 0)
            throw new ArgumentException("Maximum fine must be positive.", nameof(maximumFine));

        _baseRatePerDay = baseRatePerDay;
        _maximumFine = maximumFine;
    }

    public Fine Calculate(int daysOverdue, MembershipTier tier, ItemType itemType)
    {
        if (daysOverdue <= 0) return Fine.Zero;

        var rate = _baseRatePerDay * GetItemTypeMultiplier(itemType);
        var rawFine = new Fine(rate * daysOverdue);
        var discounted = rawFine.ApplyDiscount(GetTierDiscount(tier));
        return discounted.Cap(_maximumFine);
    }

    private static decimal GetItemTypeMultiplier(ItemType itemType) => itemType switch
    {
        ItemType.Book => 1.0m,
        ItemType.DVD => 2.0m,
        ItemType.Magazine => 0.5m,
        _ => throw new ArgumentOutOfRangeException(nameof(itemType))
    };

    private static decimal GetTierDiscount(MembershipTier tier) => tier switch
    {
        MembershipTier.Standard => 0m,
        MembershipTier.Premium => 25m,
        MembershipTier.VIP => 50m,
        _ => throw new ArgumentOutOfRangeException(nameof(tier))
    };
}
