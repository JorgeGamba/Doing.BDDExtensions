# Value Object Validation

> When to use: the SUT is a constructor-validated value object with multiple valid/invalid input scenarios

## Key Points
- Production class: `ISBN(string value)` — constructor validates/normalizes ISBN-10 and ISBN-13 inputs
- Root `When()` wraps construction in `Catch.Exception` — all subclasses share it
- `_exception` field on base; each scenario asserts null or a specific type
- 2-level hierarchy: base owns When, each `When_*` child owns Given
- Different exception types per scenario (`ArgumentNullException` / `ArgumentException` / `FormatException`)
- All members expression-bodied — one-liner Given/When/Should throughout
- `_result` is only non-null for success scenarios; keep both fields on the base

## Spec Code

```csharp
using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

[TestFixture]
public class ISBNSpecs : FeatureSpecifications
{
    protected string _input;
    protected ISBN _result;
    protected Exception _exception;

    public override void When() =>
        _exception = Catch.Exception(() => _result = new ISBN(_input));

    public class When_the_input_is_a_valid_ISBN_13 : ISBNSpecs
    {
        public override void Given() => _input = "978-0-13-468599-1";

        [Test]
        public void Should_create_the_ISBN_successfully() =>
            _exception.ShouldBeNull();

        [Test]
        public void Should_strip_hyphens_from_the_value() =>
            _result.Value.ShouldBe("9780134685991");

        [Test]
        public void Should_identify_it_as_ISBN_13() =>
            _result.IsISBN13.ShouldBeTrue();
    }

    public class When_the_input_is_null : ISBNSpecs
    {
        public override void Given() => _input = null;

        [Test]
        public void Should_throw_an_ArgumentNullException() =>
            _exception.ShouldBeOfType<ArgumentNullException>();

        [Test]
        public void Should_indicate_the_parameter_name() =>
            ((ArgumentNullException)_exception).ParamName.ShouldBe("value");
    }

    // When_the_input_is_empty, When_the_input_is_a_valid_ISBN_10,
    // When_the_input_has_an_invalid_check_digit follow the same pattern
}
```
