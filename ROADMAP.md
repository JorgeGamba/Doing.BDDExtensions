# Roadmap

Ideas for future development, aligned with the project's philosophy of behavior discovery, zero dependencies, and living documentation.

## Diagnostic Output / Spec Tree Printer

A utility that takes a `FeatureSpecifications` type and prints its hierarchy as readable text:

```
FineCalculatorSpecs
  When_the_member_is_Standard
    And_the_item_is_a_Book
      ✓ Should_charge_the_base_rate_per_day
    And_the_item_is_a_DVD
      ✓ Should_charge_double_the_base_rate
```

Reinforces "living documentation" — the spec tree IS the documentation. Could be a static method or a standalone utility. Zero-dependency constraint means reflection-only.

## Framework-Agnostic Test Runner Adapters

The project claims framework independence but currently works most naturally with NUnit (`[TestFixture]`, `[Test]`). Adapter packages for xUnit and MSTest would expand reach without changing the core. This would be separate packages (e.g., `Doing.BDDExtensions.Xunit`) that provide the necessary attributes and lifecycle integration.

## Source Generator for Hierarchy Validation

A Roslyn analyzer that validates at compile time:

- No `When()` override in children that aren't error-path wrappers
- `Given()` doesn't call `base.Given()`
- Context class names follow `When_`/`And_`/`But_` conventions
- No duplicate `When()` bodies across sibling groups

Catches anti-patterns before runtime.
