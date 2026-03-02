using Doing.BDDExtensions;
using Library.Domain;
using NUnit.Framework;
using Shouldly;

namespace Library.Specs;

/// <summary>
/// Demonstrates: Value object validation, Catch.Exception, 2-level hierarchy,
/// natural language readability, clean exception specification.
/// </summary>
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
        public override void Given() =>
            _input = "978-0-13-468599-1";

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

    public class When_the_input_is_a_valid_ISBN_10 : ISBNSpecs
    {
        public override void Given() =>
            _input = "0-13-468599-X";

        [Test]
        public void Should_create_the_ISBN_successfully() =>
            _exception.ShouldBeNull();

        [Test]
        public void Should_identify_it_as_ISBN_10() =>
            _result.IsISBN13.ShouldBeFalse();
    }

    public class When_the_input_is_null : ISBNSpecs
    {
        public override void Given() =>
            _input = null;

        [Test]
        public void Should_throw_an_ArgumentNullException() =>
            _exception.ShouldBeOfType<ArgumentNullException>();

        [Test]
        public void Should_indicate_the_parameter_name() =>
            ((ArgumentNullException)_exception).ParamName.ShouldBe("value");
    }

    public class When_the_input_is_empty : ISBNSpecs
    {
        public override void Given() =>
            _input = "";

        [Test]
        public void Should_throw_an_ArgumentException() =>
            _exception.ShouldBeOfType<ArgumentException>();

        [Test]
        public void Should_explain_the_reason() =>
            _exception.Message.ShouldStartWith("ISBN cannot be empty or whitespace.");
    }

    public class When_the_input_has_an_invalid_check_digit : ISBNSpecs
    {
        public override void Given() =>
            _input = "978-0-13-468599-9";

        [Test]
        public void Should_throw_a_FormatException() =>
            _exception.ShouldBeOfType<FormatException>();

        [Test]
        public void Should_include_the_invalid_value_in_the_message() =>
            _exception.Message.ShouldContain("978-0-13-468599-9");
    }
}
