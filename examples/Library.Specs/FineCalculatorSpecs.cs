using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: Calculator with tier/type combinations, And_/But_ prefixes,
/// 3-4 level hierarchy, progressive context refinement, single When per functionality group.
/// All three parameters (daysOverdue, tier, itemType) are determinant — each affects
/// the fine amount — so the full tier × itemType matrix is covered (9 leaf scenarios).
/// Non-determinant parameters would use any valid value without separate contexts.
/// </summary>
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

    public class When_the_item_is_not_overdue : FineCalculatorSpecs
    {
        public override void Given()
        {
            _daysOverdue = 0;
            _tier = MembershipTier.Standard;
            _itemType = ItemType.Book;
        }

        [Test]
        public void Should_return_zero_fine() =>
            _result.Amount.ShouldBe(0m);
    }

    public class When_the_member_is_Standard : FineCalculatorSpecs
    {
        public override void Given()
        {
            _tier = MembershipTier.Standard;
            _daysOverdue = 10;
        }

        public class And_the_item_is_a_Book : When_the_member_is_Standard
        {
            public override void Given() =>
                _itemType = ItemType.Book;

            [Test]
            public void Should_charge_the_base_rate_per_day() =>
                _result.Amount.ShouldBe(5.00m); // 0.50 * 1.0 * 10 = 5.00
        }

        public class And_the_item_is_a_DVD : When_the_member_is_Standard
        {
            public override void Given() =>
                _itemType = ItemType.DVD;

            [Test]
            public void Should_charge_double_the_base_rate() =>
                _result.Amount.ShouldBe(10.00m); // 0.50 * 2.0 * 10 = 10.00
        }

        public class And_the_item_is_a_Magazine : When_the_member_is_Standard
        {
            public override void Given() =>
                _itemType = ItemType.Magazine;

            [Test]
            public void Should_charge_half_the_base_rate() =>
                _result.Amount.ShouldBe(2.50m); // 0.50 * 0.5 * 10 = 2.50
        }
    }

    public class When_the_member_is_Premium : FineCalculatorSpecs
    {
        public override void Given()
        {
            _tier = MembershipTier.Premium;
            _daysOverdue = 10;
        }

        public class And_the_item_is_a_Book : When_the_member_is_Premium
        {
            public override void Given() =>
                _itemType = ItemType.Book;

            [Test]
            public void Should_apply_a_25_percent_discount() =>
                _result.Amount.ShouldBe(3.75m); // 0.50 * 1.0 * 10 * 0.75 = 3.75
        }

        public class And_the_item_is_a_DVD : When_the_member_is_Premium
        {
            public override void Given() =>
                _itemType = ItemType.DVD;

            [Test]
            public void Should_apply_a_25_percent_discount_to_the_DVD_rate() =>
                _result.Amount.ShouldBe(7.50m); // 0.50 * 2.0 * 10 * 0.75 = 7.50
        }

        public class And_the_item_is_a_Magazine : When_the_member_is_Premium
        {
            public override void Given() =>
                _itemType = ItemType.Magazine;

            [Test]
            public void Should_apply_a_25_percent_discount_to_the_magazine_rate() =>
                _result.Amount.ShouldBe(1.875m); // 0.50 * 0.5 * 10 * 0.75 = 1.875
        }
    }

    public class When_the_member_is_VIP : FineCalculatorSpecs
    {
        public override void Given()
        {
            _tier = MembershipTier.VIP;
            _daysOverdue = 10;
        }

        public class And_the_item_is_a_Book : When_the_member_is_VIP
        {
            public override void Given() =>
                _itemType = ItemType.Book;

            [Test]
            public void Should_apply_a_50_percent_discount() =>
                _result.Amount.ShouldBe(2.50m); // 0.50 * 1.0 * 10 * 0.50 = 2.50
        }

        public class And_the_item_is_a_DVD : When_the_member_is_VIP
        {
            public override void Given() =>
                _itemType = ItemType.DVD;

            [Test]
            public void Should_apply_a_50_percent_discount_to_the_DVD_rate() =>
                _result.Amount.ShouldBe(5.00m); // 0.50 * 2.0 * 10 * 0.50 = 5.00
        }

        public class And_the_item_is_a_Magazine : When_the_member_is_VIP
        {
            public override void Given() =>
                _itemType = ItemType.Magazine;

            [Test]
            public void Should_apply_a_50_percent_discount_to_the_magazine_rate() =>
                _result.Amount.ShouldBe(1.25m); // 0.50 * 0.5 * 10 * 0.50 = 1.25
        }
    }

    public class When_the_fine_exceeds_the_maximum : FineCalculatorSpecs
    {
        public override void Given()
        {
            _daysOverdue = 100;
            _tier = MembershipTier.Standard;
        }

        public class And_the_item_is_a_Book : When_the_fine_exceeds_the_maximum
        {
            public override void Given() =>
                _itemType = ItemType.Book;

            [Test]
            public void Should_cap_at_the_maximum() =>
                _result.Amount.ShouldBe(25.00m); // 0.50 * 100 = 50, capped at 25
        }

        public class And_the_item_is_a_DVD : When_the_fine_exceeds_the_maximum
        {
            public override void Given() =>
                _itemType = ItemType.DVD;

            [Test]
            public void Should_also_cap_at_the_maximum() =>
                _result.Amount.ShouldBe(25.00m); // 0.50 * 2.0 * 100 = 100, capped at 25
        }
    }
}
