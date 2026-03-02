# Calculator with Multiple Input Dimensions

> When to use: a service computes a result from several independent input dimensions (type, tier, quantity) each adding one nesting level

## Key Points
- Production class: `FineCalculator.Calculate(daysOverdue, tier, itemType)` — combines base rate, item multiplier, tier discount, and cap into a Fine result
- Single `When()` on the root — same `_calculator.Calculate(...)` runs for every scenario
- One dimension per nesting level: level 2 = membership tier, level 3 = item type
- Parent `Given()` sets shared inputs (tier + days); child `Given()` sets remaining dimension
- Boundary/cap scenarios grouped under their own `When_the_fine_exceeds_the_maximum` class
- Calculator is a `readonly` field on root (constructed once), not reset per test
- All three parameters are determinant — full tier × itemType matrix (9 scenarios) is covered
- Inline comments in `Should_` methods document the formula

## Spec Code

```csharp
[TestFixture]
public class FineCalculatorSpecs : FeatureSpecifications
{
    private readonly FineCalculator _calculator = new(baseRatePerDay: 0.50m, maximumFine: 25.00m);
    protected int _daysOverdue;
    protected MembershipTier _tier;
    protected ItemType _itemType;
    protected Fine _result;

    public override void When() =>
        _result = _calculator.Calculate(_daysOverdue, _tier, _itemType);

    // When_the_item_is_not_overdue: _daysOverdue = 0 → Should_return_zero_fine

    public class When_the_member_is_Standard : FineCalculatorSpecs
    {
        public override void Given()
        {
            _tier = MembershipTier.Standard;
            _daysOverdue = 10;
        }

        public class And_the_item_is_a_Book : When_the_member_is_Standard
        {
            public override void Given() => _itemType = ItemType.Book;

            [Test]
            public void Should_charge_the_base_rate_per_day() =>
                _result.Amount.ShouldBe(5.00m); // 0.50 * 1.0 * 10
        }

        public class And_the_item_is_a_DVD : When_the_member_is_Standard
        {
            public override void Given() => _itemType = ItemType.DVD;

            [Test]
            public void Should_charge_double_the_base_rate() =>
                _result.Amount.ShouldBe(10.00m); // 0.50 * 2.0 * 10
        }

        // And_the_item_is_a_Magazine follows the same pattern (0.50 * 0.5 * 10)
    }

    // When_the_member_is_Premium (25% discount) and When_the_member_is_VIP (50%)
    // follow the same tier × itemType pattern

    public class When_the_fine_exceeds_the_maximum : FineCalculatorSpecs
    {
        public override void Given()
        {
            _daysOverdue = 100;
            _tier = MembershipTier.Standard;
        }

        public class And_the_item_is_a_Book : When_the_fine_exceeds_the_maximum
        {
            public override void Given() => _itemType = ItemType.Book;

            [Test]
            public void Should_cap_at_the_maximum() =>
                _result.Amount.ShouldBe(25.00m); // 0.50 * 100 = 50, capped at 25
        }
    }
}
```
