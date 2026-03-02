using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: Deep nesting (4-5 levels), comparison with tolerance,
/// arithmetic operations, And_/But_ prefixes, progressive context refinement.
/// </summary>
[TestFixture]
public class FineSpecs : FeatureSpecifications
{
    protected Fine _left;
    protected Fine _right;
    protected Fine _result;
    protected bool _comparisonResult;
    protected Exception _exception;

    public class When_creating : FineSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _result = new Fine(_left?.Amount ?? 0));

        public class And_the_amount_is_positive : When_creating
        {
            public override void Given() =>
                _left = new Fine(5.25m);

            [Test]
            public void Should_create_successfully() =>
                _exception.ShouldBeNull();

            [Test]
            public void Should_preserve_the_amount() =>
                _result.Amount.ShouldBe(5.25m);
        }

        public class And_the_amount_is_zero : When_creating
        {
            public override void Given() =>
                _left = Fine.Zero;

            [Test]
            public void Should_create_successfully() =>
                _exception.ShouldBeNull();

            [Test]
            public void Should_have_zero_amount() =>
                _result.Amount.ShouldBe(0m);
        }

        public class But_the_amount_is_negative : When_creating
        {
            public override void Given() =>
                _left = null;

            public override void When() =>
                _exception = Catch.Exception(() => new Fine(-1m));

            [Test]
            public void Should_throw_an_ArgumentException() =>
                _exception.ShouldBeOfType<ArgumentException>();
        }
    }

    public class When_comparing_for_equality : FineSpecs
    {
        public override void Given() =>
            _left = new Fine(4.50m);

        public override void When() =>
            _comparisonResult = _left.Equals(_right);

        public class And_both_are_exactly_equal : When_comparing_for_equality
        {
            public override void Given() =>
                _right = new Fine(4.50m);

            [Test]
            public void Should_be_equal() =>
                _comparisonResult.ShouldBeTrue();
        }

        public class And_both_are_within_tolerance : When_comparing_for_equality
        {
            // Tolerance is 0.001, so 4.5009 is within tolerance of 4.50
            public override void Given() =>
                _right = new Fine(4.5009m);

            [Test]
            public void Should_be_equal() =>
                _comparisonResult.ShouldBeTrue();
        }

        public class And_both_are_outside_tolerance : When_comparing_for_equality
        {
            public override void Given() =>
                _right = new Fine(4.502m);

            [Test]
            public void Should_not_be_equal() =>
                _comparisonResult.ShouldBeFalse();
        }

        public class And_the_other_is_null : When_comparing_for_equality
        {
            public override void Given() =>
                _right = null;

            [Test]
            public void Should_not_be_equal() =>
                _comparisonResult.ShouldBeFalse();
        }
    }

    public class When_applying_a_discount : FineSpecs
    {
        protected decimal _discountPercent;

        public override void Given() =>
            _left = new Fine(20.00m);

        public override void When() =>
            _result = _left.ApplyDiscount(_discountPercent);

        public class And_the_discount_is_25_percent : When_applying_a_discount
        {
            public override void Given() =>
                _discountPercent = 25m;

            [Test]
            public void Should_reduce_the_fine_accordingly() =>
                _result.Amount.ShouldBe(15.00m);
        }

        public class And_the_discount_is_50_percent : When_applying_a_discount
        {
            public override void Given() =>
                _discountPercent = 50m;

            [Test]
            public void Should_halve_the_fine() =>
                _result.Amount.ShouldBe(10.00m);
        }

        public class And_the_discount_is_100_percent : When_applying_a_discount
        {
            public override void Given() =>
                _discountPercent = 100m;

            [Test]
            public void Should_result_in_zero() =>
                _result.Amount.ShouldBe(0m);
        }

        public class But_the_discount_exceeds_100_percent : When_applying_a_discount
        {
            public override void When() =>
                _exception = Catch.Exception(() => _left.ApplyDiscount(101m));

            [Test]
            public void Should_throw_an_ArgumentOutOfRangeException() =>
                _exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }
    }

    public class When_capping_a_fine : FineSpecs
    {
        protected decimal _capAmount;

        public override void Given() =>
            _left = new Fine(50.00m);

        public override void When() =>
            _result = _left.Cap(_capAmount);

        public class And_the_fine_exceeds_the_cap : When_capping_a_fine
        {
            public override void Given() =>
                _capAmount = 25.00m;

            [Test]
            public void Should_reduce_to_the_cap() =>
                _result.Amount.ShouldBe(25.00m);
        }

        public class And_the_fine_is_under_the_cap : When_capping_a_fine
        {
            public override void Given() =>
                _capAmount = 100.00m;

            [Test]
            public void Should_remain_unchanged() =>
                _result.Amount.ShouldBe(50.00m);
        }
    }
}
