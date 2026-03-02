namespace Library.Domain;

public class ISBN
{
    public string Value { get; }
    public bool IsISBN13 { get; }

    public ISBN(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ISBN cannot be empty or whitespace.", nameof(value));

        var digits = value.Replace("-", "").Replace(" ", "");

        if (digits.Length == 13 && IsValidISBN13(digits))
        {
            Value = digits;
            IsISBN13 = true;
        }
        else if (digits.Length == 10 && IsValidISBN10(digits))
        {
            Value = digits;
            IsISBN13 = false;
        }
        else
        {
            throw new FormatException($"'{value}' is not a valid ISBN-10 or ISBN-13.");
        }
    }

    public override bool Equals(object obj) =>
        obj is ISBN other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    private static bool IsValidISBN13(string digits)
    {
        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            if (!char.IsDigit(digits[i])) return false;
            sum += (digits[i] - '0') * (i % 2 == 0 ? 1 : 3);
        }
        var checkDigit = (10 - sum % 10) % 10;
        return char.IsDigit(digits[12]) && (digits[12] - '0') == checkDigit;
    }

    private static bool IsValidISBN10(string digits)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            if (!char.IsDigit(digits[i])) return false;
            sum += (digits[i] - '0') * (10 - i);
        }
        var lastChar = digits[9];
        sum += lastChar == 'X' ? 10 : (char.IsDigit(lastChar) ? (lastChar - '0') : -1);
        return sum % 11 == 0;
    }
}
