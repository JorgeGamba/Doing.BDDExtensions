# Anti-Patterns

> Common mistakes when writing BDD specifications with Doing.BDDExtensions

## Structural Anti-Patterns

### Flat Instead of Nested Hierarchy
- **Problem**: Testing interacting parameter combinations as flat `When_` siblings instead of nesting
- **Fix**: Identify determinant parameters, compose hierarchically — one dimension per nesting level
- Variants:
  - **Flat configuration parameters** — flat `When_` siblings for each parameter value instead of nested dimensions
  - **Compound state context** — merging parameters into one name (`When_both_X_and_Y`); masks missing combinations. Detection: `both`, `and`, `neither` in context names
  - **Flat scenarios without combinatorial coverage** — testing tier and item type as separate flat lists (3+3) instead of composed (3×3)
  - **Flat scenarios that should be nested** — multiple `When_` siblings testing variations of the same parameter
- See [unit-configuration-builder](../examples/unit-configuration-builder.md) for WRONG vs CORRECT comparison

### And_/But_ at Root Level
- **Problem**: Using `And_`/`But_` prefix for direct children of root spec class
- **Fix**: Root-level children always use `When_` prefix; `And_`/`But_` only at level 2+

### Multiple When() Within a Functionality Group
- **Problem**: Children overriding the group's `When()` to change the operation (not for `Catch.Exception` wrapping)
- **Fix**: Use `Given()` to vary input parameters; let the group's `When()` call the operation
- Note: Multiple `When_` groups at level 2 with each their own `When()` IS valid for multi-method classes
- Variant: **Duplicated identical When() across sibling groups** — each `When_` group defines the same `When()` body instead of defining it once at their common ancestor. Detection: two or more `When()` overrides with the same expression body. Fix: move `When()` to the highest shared level; children vary context via `Given()` only

### Flat Test Class (No Hierarchy)
- **Problem**: All tests at one level with no nesting
- **Fix**: Group related tests under `When_`, `And_`, `But_` contexts

### Too-Deep Nesting (6+ Levels)
- **Problem**: Hierarchy so deep it's hard to follow; likely mixing concerns
- **Fix**: Extract a separate spec file for a sub-behavior; 4-5 levels is acceptable when driven by combinatorial coverage

### Breadth-Over-Depth in Large Scope
- **Problem**: One shallow `When_` per API method instead of deep combinatorial hierarchies per feature area
- **Fix**: Decompose into independent feature areas; apply full Scenario Discovery per area independently
- **Detection**: If your tree has no `And_`/`But_` nesting and every `When_` is flat — you skipped Scenario Discovery

### Multi-Statement When()
- **Problem**: `When()` contains setup + SUT invocation (multiple statements)
- **Fix**: Move all setup to `Given()`; make `When()` an expression-bodied single SUT invocation

### TODO Comments Instead of Pending Tests
- **Problem**: Using `// TODO: add VIP scenarios` comments for deferred scenarios
- **Fix**: Create the class structure with `[Ignore("Deferred: reason")]` — compiles, visible in test runner as "Skipped", refactoring catches renames. See SKILL.md §2 Deferred Scenarios

### Inheriting from Project Base Class
- **Problem**: Spec inherits from a project-specific base class that overrides `Given()`
- **Fix**: Use composition-based context class; inherit only from `FeatureSpecifications`

## Naming Anti-Patterns

### Generic Names
- **Bad**: `When_case_1`, `Should_work`, `Should_be_correct`
- **Good**: `When_the_member_has_overdue_fines`, `Should_apply_a_25_percent_discount`

### Test-Think Instead of Spec-Think
- **Bad**: `TestCalculation`, `Verify_output_is_right`
- **Good**: `When_calculating_for_a_premium_member`, `Should_apply_a_50_percent_discount`

### Consequence-Based Context Naming
- **Problem**: `When_calculating_for_a_standard_member` (describes consequence, not state)
- **Fix**: `When_the_member_is_Standard` — name the domain state `Given()` configures

## Setup Anti-Patterns

### Given() Repeats Parent Setup
- **Fix**: Only set what's DIFFERENT from parent; parent→child execution is guaranteed

### Given() Does Too Much
- **Fix**: Move shared setup to root `Given()`; vary specific fields in child `Given()`

### Using [SetUp] or Constructor Instead of Given()
- **Fix**: Always use `Given()` — `[SetUp]` and constructors don't participate in the reflection-based hierarchy walk

### Private Fields Instead of Protected
- **Fix**: Use `protected` for all shared state fields — nested child classes need access

## Assertion Anti-Patterns

### Multiple Assertions in One Test
- **Fix**: One assertion per `Should_*` method; use multiple test methods

### Mixing Assertion Styles
- **Fix**: Pick one assertion library per project and use it consistently

### Testing Implementation Instead of Behavior
- **Fix**: Assert return values and side effects through public API; `Received()` is acceptable at module level

## Formatting Anti-Patterns

### Lambda Body on Same Line
- **Fix**: Always break after `=>` — declaration on one line, expression body on next line indented

## File Organization Anti-Patterns

### Specs in Wrong Folder
- **Fix**: Match folder structure exactly — `Services/PaymentService.cs` → `Services/PaymentServiceSpecs.cs`

### Multiple Production Classes per Spec File
- **Fix**: One spec file per production class, always

### Shared Test Utilities in Spec Files
- **Fix**: Extract to separate files: `ObjectMother.cs`, `TestContext.cs`, `TestHelpers.cs`
