# Doing.BDDExtensions

[![NuGet](https://img.shields.io/nuget/v/Doing.BDDExtensions.svg)](https://www.nuget.org/packages/Doing.BDDExtensions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A minimal, zero-dependency BDD specification framework for .NET. Write **behavior specifications** — not tests — that serve as living documentation and regression safety nets.

## What Is This?

Doing.BDDExtensions is a **behavior discovery tool**, not a testing library. You write specifications of what the system *should do*, organized as hierarchical contexts that read as natural language:

```
FineCalculatorSpecs
  When the member is Standard
    And the item is a Book
      ✓ Should charge the base rate per day
    And the item is a DVD
      ✓ Should charge double the base rate
  When the fine exceeds the maximum
    ✓ Should cap at the maximum
```

The output is **living documentation** that also happens to function as regression tests.

### Why Not Traditional Tests?

| Aspect | Traditional Tests (AAA) | BDD Specifications |
|--------|------------------------|--------------------|
| **Purpose** | Verify code works | Discover & document behavior |
| **Structure** | Flat methods in a class | Hierarchical contexts via inheritance |
| **Readability** | Requires reading code | Reads as sentences |
| **Setup sharing** | Repeated or fragile helpers | Inherited via parent→child chain |
| **Naming** | `Test_Calculate_Returns5` | `When_calculating / Should_return_5` |
| **Navigation** | Arbitrary file organization | 1:1 mapping to production code |
| **Documentation** | Separate from code | The specs *are* the documentation |

## Installation

```bash
dotnet add package Doing.BDDExtensions
```

The library targets **.NET Standard 2.0** with zero dependencies — just `System` and `System.Reflection`. Compatible with any .NET project (.NET Framework 4.6.1+, .NET Core, .NET 5+).

For test assertions, we recommend [Shouldly](https://github.com/shouldly/shouldly):

```bash
dotnet add package Shouldly
```

## Quick Start

Given a production class:

```csharp
public class ISBN
{
    public string Value { get; }

    public ISBN(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ISBN cannot be empty.", nameof(value));
        Value = value.Replace("-", "");
    }
}
```

Write a specification:

```csharp
using Doing.BDDExtensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public class ISBNSpecs : FeatureSpecifications
{
    protected string _input;
    protected ISBN _result;
    protected Exception _exception;

    public override void When() =>
        _exception = Catch.Exception(() => _result = new ISBN(_input));

    public class When_the_input_is_valid : ISBNSpecs
    {
        public override void Given() =>
            _input = "978-0-13-468599-1";

        [Test]
        public void Should_create_successfully() =>
            _exception.ShouldBeNull();

        [Test]
        public void Should_strip_hyphens() =>
            _result.Value.ShouldBe("9780134685991");
    }

    public class When_the_input_is_null : ISBNSpecs
    {
        public override void Given() =>
            _input = null;

        [Test]
        public void Should_throw_ArgumentNullException() =>
            _exception.ShouldBeOfType<ArgumentNullException>();
    }
}
```

Run with:

```bash
dotnet test
```

## Key Concepts

### `FeatureSpecifications` Base Class

Every spec inherits from `FeatureSpecifications`. It provides:

- **`Given()`** — Override to set up preconditions (context)
- **`When()`** — Override to perform the action being specified
- **Test methods** (`[Test] Should_*()`) — Assert expected outcomes (Then)

```csharp
public class MyServiceSpecs : FeatureSpecifications
{
    public override void Given() { /* setup */ }
    public override void When()  { /* action */ }

    [Test]
    public void Should_produce_expected_result() { /* assertion */ }
}
```

### Hierarchical Context Nesting

The defining feature: nest classes to create context trees. Each level can override `Given()` to add preconditions. **Parent `Given()` methods execute first** (parent→child order), guaranteed by reflection.

```csharp
public class FineCalculatorSpecs : FeatureSpecifications
{
    // Level 0: Root — shared setup
    public override void Given() { /* create calculator, set defaults */ }
    public override void When() { /* call Calculate() */ }

    public class When_the_member_is_Standard : FineCalculatorSpecs
    {
        // Level 1: Set tier
        public override void Given() { _tier = MembershipTier.Standard; }

        public class And_the_item_is_a_DVD : When_the_member_is_Standard
        {
            // Level 2: Set item type (parent Given()s already set tier)
            public override void Given() { _itemType = ItemType.DVD; }

            [Test]
            public void Should_charge_double_the_base_rate() =>
                _result.Amount.ShouldBe(10.00m);
        }
    }
}
```

**Execution order** for `And_the_item_is_a_DVD`:
1. `FineCalculatorSpecs.Given()` — creates calculator, sets defaults
2. `When_the_member_is_Standard.Given()` — sets tier
3. `And_the_item_is_a_DVD.Given()` — sets item type
4. `FineCalculatorSpecs.When()` — calls Calculate()
5. `Should_charge_double_the_base_rate()` — asserts result

### `Catch.Exception()`

Capture exceptions for assertion instead of letting them fail the test:

```csharp
public override void When() =>
    _exception = Catch.Exception(() => new ISBN(null));

[Test]
public void Should_throw_ArgumentNullException() =>
    _exception.ShouldBeOfType<ArgumentNullException>();

[Test]
public void Should_indicate_the_parameter() =>
    ((ArgumentNullException)_exception).ParamName.ShouldBe("value");
```

An async overload is available for `Task`-returning operations:

```csharp
public override void When() =>
    _exception = Catch.Exception(async () => await _service.ProcessAsync(null));
```

The async overload automatically unwraps `AggregateException` when it contains a single inner exception, surfacing the original exception directly.

### Single When() Per Functionality Group

Each spec file specifies behavior for **one production class**. When the class has a single public method, there's one `When()` at the root. When the class exposes multiple operations, each gets its own `When_` group at level 2, each with its own `When()`:

- `FineCalculator.Calculate()` → `FineCalculatorSpecs` with one root `When()`
- `Fine` (create, compare, discount, cap) → `FineSpecs` with `When_creating`, `When_comparing_for_equality`, `When_applying_a_discount`, `When_capping_a_fine` — each group owns its `When()`
- `BookLifecycle` (lend, return, mark overdue, report lost, fulfill) → `BookLifecycleSpecs` with one `When_` per method

Within a functionality group, children inherit the group's `When()` — they only override `Given()` to vary conditions. The sole exception: error-path children may override `When()` to wrap with `Catch.Exception()`.

`When()` should normally be an expression-bodied member — a single invocation of the SUT's public method that captures the result. Any setup needed before the invocation belongs in `Given()`, not `When()`.

This creates natural navigation: to find specs for `LendingService.Borrow()`, open `LendingServiceSpecs.cs`.

## Fractal Spec Composition

The same Context/Specification structure — nested classes with Given/When/Then — repeats at every level of the test pyramid. The pattern is **fractal**: self-similar at every scale.

What varies across levels is nesting depth, infrastructure complexity, and quantity — not the approach.

### Delegation Principle

Each level stubs/mocks the level below at its **happy path** and focuses its own scenarios on its own responsibility:

```csharp
// Orchestrator level: mocks factory at happy-path, tests coordination
[TestFixture]
public class LendingServiceSpecs : FeatureSpecifications
{
    public override void Given()
    {
        _bookRepository = Substitute.For<IBookRepository>();
        _bookRepository.GetState(_bookId).Returns(BookState.Available); // happy-path default
        // ... wire service with mocked dependencies
    }

    public override void When() =>
        _result = _service.Borrow(_member, _bookId, _requestDate);

    public class When_the_book_is_available_and_member_is_eligible : LendingServiceSpecs
    {
        [Test]
        public void Should_approve_the_loan() =>
            _result.ShouldBeOfType<LoanApproval>();

        [Test]
        public void Should_update_the_book_state_to_OnLoan() =>
            _bookRepository.Received().UpdateState(_bookId, BookState.OnLoan);
    }

    public class When_the_book_is_not_available : LendingServiceSpecs
    {
        public override void Given() =>
            _bookRepository.GetState(_bookId).Returns(BookState.OnLoan);

        [Test]
        public void Should_reject_the_loan() =>
            _result.ShouldBeOfType<LoanRejection>();
    }
}

// Unit level: exhaustive determinant parameter combinations
[TestFixture]
public class FineCalculatorSpecs : FeatureSpecifications
{
    public override void When() =>
        _result = _calculator.Calculate(_daysOverdue, _tier, _itemType);

    public class When_the_member_is_Standard : FineCalculatorSpecs
    {
        public override void Given() { _tier = MembershipTier.Standard; _daysOverdue = 10; }

        public class And_the_item_is_a_Book : When_the_member_is_Standard
        {
            public override void Given() =>
                _itemType = ItemType.Book;

            [Test]
            public void Should_charge_the_base_rate_per_day() =>
                _result.Amount.ShouldBe(5.00m);
        }
        // And_the_item_is_a_DVD, And_the_item_is_a_Magazine ...
    }
    // When_the_member_is_Premium, When_the_member_is_VIP ...
}
```

Edge cases and error scenarios at each level are for **that level's** responsibility. The detailed variations of delegated behavior belong to the lower level's specs.

### Acceptance-Level Considerations

For specs involving infrastructure (HTTP endpoints, databases, auth):
- Root `Given()` creates infrastructure: test server, seed data, authentication context
- Child `Given()` overrides ONE condition: remove auth, change input, modify state
- `[TearDown]` restores shared mutable state (e.g., authentication) after mutation

## Examples

The [`examples/`](examples/) folder contains a complete **Library System** domain demonstrating all specification patterns:

| Level | Spec | Pattern |
|-------|------|---------|
| Unit | [ISBNSpecs](examples/Library.Specs/ISBNSpecs.cs) | Value object validation + exception specification |
| Unit | [FineSpecs](examples/Library.Specs/FineSpecs.cs) | Deep nesting (4+ levels), tolerance comparison |
| Unit | [FineCalculatorSpecs](examples/Library.Specs/FineCalculatorSpecs.cs) | Calculator with tier/type dimension combinations |
| Service | [LoanEligibilityEvaluatorSpecs](examples/Library.Specs/LoanEligibilityEvaluatorSpecs.cs) | Stateful evaluator with multiple condition paths |
| Service | [LoanRequestFactorySpecs](examples/Library.Specs/LoanRequestFactorySpecs.cs) | Polymorphic returns (Approval vs Rejection) |
| Module | [LendingServiceSpecs](examples/Library.Specs/LendingServiceSpecs.cs) | Orchestration with mock collaborators |
| Module | [BookLifecycleSpecs](examples/Library.Specs/BookLifecycleSpecs.cs) | State machine transitions |

Each spec file maps 1:1 to a production class in [`examples/Library.Domain/`](examples/Library.Domain/).

## Specification Patterns

### Exception Specification with Catch

Use `Catch.Exception()` to specify expected failures:

```csharp
public override void When() =>
    _exception = Catch.Exception(() => _book.Lend());

public class And_the_book_is_Available : When_lending
{
    public override void Given() =>
        _book = new BookLifecycle(BookState.Available);

    [Test]
    public void Should_not_throw() =>
        _exception.ShouldBeNull();
}

public class But_the_book_is_already_OnLoan : When_lending
{
    public override void Given() =>
        _book = new BookLifecycle(BookState.OnLoan);

    [Test]
    public void Should_throw_InvalidOperationException() =>
        _exception.ShouldBeOfType<InvalidOperationException>();
}
```

### Polymorphic Return Specification

When a method returns different types based on conditions:

```csharp
protected LoanApproval ResultAsApproval => (LoanApproval)_result;
protected LoanRejection ResultAsRejection => (LoanRejection)_result;

public class When_the_member_is_eligible : LoanRequestFactorySpecs
{
    [Test]
    public void Should_produce_a_LoanApproval() =>
        _result.ShouldBeOfType<LoanApproval>();

    [Test]
    public void Should_set_the_due_date() =>
        ResultAsApproval.DueDate.ShouldBe(new DateTime(2024, 6, 15));
}
```

### State Machine Specification

Map state transitions to spec hierarchy:

```csharp
public class When_lending_a_book : BookLifecycleSpecs
{
    public override void When() =>
        _exception = Catch.Exception(() => _book.Lend());

    public class And_the_book_is_Available : When_lending_a_book
    {
        public override void Given() =>
            _book = new BookLifecycle(BookState.Available);

        [Test]
        public void Should_transition_to_OnLoan() =>
            _book.CurrentState.ShouldBe(BookState.OnLoan);
    }

    public class But_the_book_is_already_OnLoan : When_lending_a_book
    {
        public override void Given() =>
            _book = new BookLifecycle(BookState.OnLoan);

        [Test]
        public void Should_throw_InvalidOperationException() =>
            _exception.ShouldBeOfType<InvalidOperationException>();
    }
}
```

## Async Support

`Given()` and `When()` support `async void` transparently. The framework detects async operations and blocks until all continuations complete before proceeding to the next step. Existing synchronous specs are unaffected.

```csharp
[TestFixture]
public class BookApiSpecs : FeatureSpecifications
{
    protected HttpClient _client;
    protected HttpResponseMessage _response;

    public override void Given() =>
        _client = new TestServer().Client;

    public override async void When() =>
        _response = await _client.GetAsync("/books/1");

    public class When_the_book_exists : BookApiSpecs
    {
        [Test]
        public void Should_return_200_OK() =>
            ((int)_response.StatusCode).ShouldBe(200);
    }
}
```

The execution order is preserved: parent `Given()` → child `Given()` → `When()`, even when any of them are async. Each step completes fully before the next one begins.

## API Reference

### `FeatureSpecifications` (abstract class)

```csharp
public abstract class FeatureSpecifications
{
    public virtual void Given();   // Override to set up preconditions
    public virtual void When();    // Override to perform the action
}
```

- **Constructor** calls `Given()` then `When()` automatically
- **`Given()` execution order**: parent→child (guaranteed by reflection-based hierarchy walk)
- **Intermediate levels without `Given()`** are skipped gracefully
- Inherit at any depth for context nesting

### `Catch` (static class)

```csharp
public static class Catch
{
    public static Exception Exception(Action action);
    public static Exception Exception(Func<Task> action);
}
```

- Returns the caught `Exception`, or `null` if no exception occurred
- Use for specifying expected failures without try/catch boilerplate
- The `Func<Task>` overload blocks synchronously and unwraps single-inner `AggregateException`

## AI-Assisted Spec Writing

This project includes a **`behavior-specs`** skill for [Claude Code](https://claude.com/claude-code) that assists in planning, creating, and maintaining BDD specifications.

### For Contributors

The skill is included in the repository at `.claude/skills/behavior-specs/`. Clone the repo and the skill is automatically available.

### For NuGet Package Users

To install the skill in your project:

```bash
# From your project root
mkdir -p .claude/skills
cp -r path/to/Doing.BDDExtensions/.claude/skills/behavior-specs .claude/skills/
```

Or clone just the skill folder from GitHub:

```bash
# Download the skill
git clone --depth 1 --filter=blob:none --sparse https://github.com/JorgeGamba/Doing.BDDExtensions.git /tmp/bdd-skill
cd /tmp/bdd-skill
git sparse-checkout set .claude/skills/behavior-specs

# Copy to your project
cp -r .claude/skills/behavior-specs /path/to/your/project/.claude/skills/
```

### What the Skill Provides

- **4 prompts**: Create specs, add context levels, review specs
- **9 examples**: Patterns from unit to acceptance level
- **3 references**: Naming conventions, pattern catalog, anti-patterns
- Triggers on: "create spec", "add spec", "BDD", "Given/When/Then", "behavior spec"

## Contributing

Contributions are welcome! When adding features:

1. Write specifications first (BDD-style, using the framework itself)
2. Ensure existing specs pass: `dotnet test src/Doing.BDDExtensions.Specs/Doing.BDDExtensions.Specs.csproj`
3. Maintain zero dependencies in the core library
4. Follow the naming conventions in this README

## License

[MIT](LICENSE) - Copyright 2017 (c) Jorge Gamba
