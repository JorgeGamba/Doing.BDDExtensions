namespace Library.Domain;

public class Fine : IEquatable<Fine>, IComparable<Fine>
{
    private const decimal Tolerance = 0.001m;

    public decimal Amount { get; }

    public Fine(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Fine amount cannot be negative.", nameof(amount));
        Amount = amount;
    }

    public static Fine Zero => new(0m);

    public Fine Add(Fine other) => new(Amount + other.Amount);

    public Fine Subtract(Fine other)
    {
        var result = Amount - other.Amount;
        return new Fine(result < 0 ? 0 : result);
    }

    public Fine ApplyDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Discount must be between 0 and 100.");
        return new Fine(Amount * (1 - percentage / 100));
    }

    public Fine Cap(decimal maximumAmount) =>
        Amount > maximumAmount ? new Fine(maximumAmount) : this;

    public bool Equals(Fine other)
    {
        if (other is null) return false;
        return Math.Abs(Amount - other.Amount) < Tolerance;
    }

    public override bool Equals(object obj) => Equals(obj as Fine);

    public override int GetHashCode() => Math.Round(Amount, 2).GetHashCode();

    public int CompareTo(Fine other)
    {
        if (other is null) return 1;
        if (Equals(other)) return 0;
        return Amount.CompareTo(other.Amount);
    }

    public static bool operator ==(Fine left, Fine right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Fine left, Fine right) => !(left == right);

    public static bool operator >(Fine left, Fine right) => left.CompareTo(right) > 0;

    public static bool operator <(Fine left, Fine right) => left.CompareTo(right) < 0;

    public override string ToString() => $"${Amount:F2}";
}
