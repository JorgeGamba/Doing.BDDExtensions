# Naming Conventions

> Doing.BDDExtensions framework conventions extracted from codebase + reference projects

## File Naming

| Artifact | Pattern | Example |
|----------|---------|---------|
| Spec file | `{ProductionClassName}Specs.cs` | `FineCalculatorSpecs.cs` |
| Spec folder | Mirror production structure | `Services/` → `Services/` |
| One spec file per production class | Always | Never combine specs for multiple classes |
| Multiple `When_` groups per file | Valid for multi-method classes | `FineSpecs` has 4 groups |

## Class Naming

| Level | Pattern | Example |
|-------|---------|---------|
| Root spec class | `{ProductionClass}Specs` | `FineCalculatorSpecs` |
| Action group | `When_{domain_state}` | `When_the_member_is_Standard` |
| Added condition | `And_{condition}` | `And_the_item_is_a_DVD` |
| Contrasting condition | `But_{condition}` | `But_the_amount_is_negative` |
| Cause/reason | `Because_{reason}` | `Because_the_member_is_suspended` |

Rules:
- Use underscores between words (reads as natural language)
- **Scenario as state transition**: Given = precondition state, When = transition trigger, Then = new state. Each scenario specifies a **behaviour change**
- Context names express **domain state**, not consequence — `When_the_member_is_Standard` not `When_calculating_for_a_standard_member`
- `And_`/`But_` are level 2+ only — root-level children always use `When_`
- Nesting is the default; flat `When_` siblings only for truly independent operations
- Combinatorial coverage only for **determinant** parameters; non-determinant use any valid value
- Avoid redundant nouns already implied by the file/root class name

### Context Name Validation

Apply to every `When_`/`And_`/`But_` name: **does it answer "what condition exists?" (correct) or "what happens?" (wrong)**.

| Wrong (consequence) | Right (domain state) |
|---|---|
| `When_calculating_for_a_standard_member` | `When_the_member_is_Standard` |
| `When_a_module_uses_the_default_name` | `When_no_name_is_configured` |
| `And_the_discount_is_applied` | `And_the_member_is_Premium` |
| `When_requesting_a_protected_endpoint` | `When_the_endpoint_requires_authentication` |
| `When_requesting_a_nonexistent_route` | `When_the_route_does_not_exist` |
| `When_an_endpoint_throws_an_exception` | `When_the_endpoint_throws_an_exception` |

**Verification**: Read the `Given()` — the class name should describe what `Given()` configures, not what `When()` produces.

## Test Method Naming

| Pattern | Example |
|---------|---------|
| `Should_{expected_outcome}` | `Should_apply_a_25_percent_discount` |
| `Should_{verb}_{noun}` | `Should_return_zero_fine` |
| `Should_throw_{ExceptionType}` | `Should_throw_an_ArgumentException` |
| `Should_not_{verb}_{noun}` | `Should_not_update_the_book_state` |

Rules:
- Always start with `Should_`; be specific (`Should_be_eligible` not `Should_work`)
- Include expected values when relevant: `Should_cap_at_the_maximum`
- **Fractal correspondence**: when a `Should_` describes behavior delegated to a lower level, the domain concept should be traceable to a `When_` context at that level (see SKILL.md §4 Fractal Naming)

## Field Naming

| Type | Pattern | Example |
|------|---------|---------|
| Private fields | `_camelCase` | `_result`, `_exception`, `_member` |
| SUT | `_sut` or `_{descriptiveName}` | `_calculator`, `_service` |
| Mocks | `_{interfaceName}` (without I) | `_bookRepository` for `IBookRepository` |
| Test result | `_result` | Universal convention |
| Captured exception | `_exception` | Universal convention |

Rules:
- All fields `protected` (accessible to nested subclasses)
- Declare at the **highest level where first relevant** — don't push to root if only one `When_` uses it

## Lambda Formatting

Always break after `=>` to next line in `Given()`/`When()`/`Should_` methods.

## Hierarchy Depth

| Spec Level | Typical Depth | Example |
|------------|---------------|---------|
| Unit (value object) | 2 levels | `ISBNSpecs → When_the_input_is_null` |
| Unit (calculator) | 3-4 levels | `FineCalculatorSpecs → When_the_member_is_Standard → And_the_item_is_a_DVD` |
| Service (factory) | 3 levels | `LoanRequestFactorySpecs → When_the_member_is_eligible → And_the_member_is_Premium` |
| Module (orchestrator) | 2-3 levels | `LendingServiceSpecs → When_the_book_is_available_and_member_is_eligible` |
| Application (API) | 2-3 levels | `LibraryAPISpecs → When_borrowing_a_book → And_the_book_is_available` |

Depth follows determinant parameter count, not an arbitrary limit. 4-5 levels is acceptable when scenario structure requires it.
