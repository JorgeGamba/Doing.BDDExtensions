# Deep Nesting with Multiple Operation Groups

> When to use: a value type has several distinct operations (creation, comparison, arithmetic) each requiring its own sub-scenarios

## Key Points
- Production class: `Fine` — immutable value object with tolerance-based equality, discount, and cap operations
- Each operation group (`When_creating`, `When_comparing_for_equality`, etc.) lives at level 2 with its own `When()`
- `And_` = additive sub-case (same polarity); `But_` = contrasting sub-case (opposite/edge)
- Child `Given()` only sets incremental delta; parent `Given()` runs first via framework
- `Catch.Exception` at operation level, not root — only operations that can throw wrap themselves
- No root `When()` — each operation group defines its own

## Spec Code

```csharp
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
            public override void Given() => _left = new Fine(5.25m);

            [Test]
            public void Should_preserve_the_amount() => _result.Amount.ShouldBe(5.25m);
        }

        public class But_the_amount_is_negative : When_creating
        {
            public override void When() => _exception = Catch.Exception(() => new Fine(-1m));

            [Test]
            public void Should_throw_an_ArgumentException() =>
                _exception.ShouldBeOfType<ArgumentException>();
        }
    }

    public class When_comparing_for_equality : FineSpecs
    {
        public override void Given() => _left = new Fine(4.50m);
        public override void When() => _comparisonResult = _left.Equals(_right);

        public class And_both_are_exactly_equal : When_comparing_for_equality
        {
            public override void Given() => _right = new Fine(4.50m);

            [Test]
            public void Should_be_equal() => _comparisonResult.ShouldBeTrue();
        }

        public class And_both_are_within_tolerance : When_comparing_for_equality
        {
            public override void Given() => _right = new Fine(4.5009m);

            [Test]
            public void Should_be_equal() => _comparisonResult.ShouldBeTrue();
        }

        // And_both_are_outside_tolerance, And_the_other_is_null follow same pattern
    }

    // When_applying_a_discount (And_25%, And_50%, And_100%, But_exceeds_100%)
    // and When_capping_a_fine (And_exceeds_cap, And_under_cap)
    // follow the same operation-group pattern
}
```
