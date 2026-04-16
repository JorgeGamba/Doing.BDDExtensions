---
name: behavior-specs
description: >
  Assist in planning, creating, and maintaining BDD specifications using
  Doing.BDDExtensions. Triggers: "create spec", "add spec", "specification for",
  "BDD", "Given/When/Then", "context/spec", "behavior spec", "write spec".
model: opus
---

# behavior-specs

Create and maintain BDD specifications using the Doing.BDDExtensions framework. This is a **behavior discovery** tool — specs document what the system should do, producing living documentation and regression tests as a side-effect.

## 0. Detect Project Testing Stack

Before writing any spec, detect the project's testing tools. The BDD methodology is universal; frameworks are implementation details.

**Detection order**: CLAUDE.md → test `.csproj` `<PackageReference>` entries → existing spec file `using` statements → ask user

**Known packages**: NUnit/xUnit/MSTest (runner), Shouldly/FluentAssertions/NUnit Assert (assertions), NSubstitute/Moq/FakeItEasy (mocking)

**What adapts**: `using` imports, test attributes, assertion syntax, mock creation syntax

**What does NOT adapt** (universal): `FeatureSpecifications` base class, `Catch.Exception()`, scenario discovery methodology, hierarchy structure, naming conventions, all rules in this skill

## 1. Understand the Target

Before writing any spec:
- Identify the **production class** and its **single public responsibility**
- Determine the **spec level**:

| Level | Characteristics | Depth | Mocks |
|-------|----------------|-------|-------|
| Unit | Value objects, calculators, small functions | Deep nesting | None |
| Service | Factories, evaluators, stateful services | Moderate | May test polymorphic returns |
| Module | Coordinators, orchestrators | Shallow | Mock collaborators, verify interactions |
| Application | API endpoints, full workflows | 2-3 levels | Real infrastructure |

Favor more specs at the base (Unit/Service) and fewer at the top (Application).

### Assess Domain Complexity

Before committing to full Scenario Discovery, assess domain uncertainty (Keogh's Cynefin mapping):

| Domain | Strategy |
|--------|----------|
| **Obvious/Complicated** | Full Scenario Discovery (§2); automate all specs |
| **Complex** (high uncertainty) | Probe first — write 2-3 key scenarios to learn, then iterate; specs as experiment design |
| **Chaotic** | Stabilize first; specify after |

Most Unit/Service specs are Obvious/Complicated. Module/Application specs in new domains may be Complex — probe before committing to full coverage.

### GWT Scope Decision

GWT nesting is appropriate when parameters have **domain meaning** (tier: Standard vs Premium changes logic). Evaluate alternatives when:
- Action is a **pure numeric calculation** with no domain-logic branching → consider `[TestCase]`/`[TestCaseSource]` tabular format
- Parameters are **value permutations** without behavioral difference → tabular is more readable than nesting

GWT nesting remains correct (and preferred) when the parameter names and states carry domain semantics that read naturally as a specification hierarchy.

## 2. Design the Specification Hierarchy

Every spec file follows this structure:

```
RootSpecs : FeatureSpecifications          ← Feature/class being specified
├── Given()                                ← Shared setup (dependencies, SUT, defaults)
├── When()                                 ← The single action being specified
├── When_scenario_A : RootSpecs            ← First scenario group
│   ├── Given()                            ← Refines parent context
│   ├── And_condition_1 : When_scenario_A  ← Condition variation
│   │   ├── Given()                        ← Adds specific precondition
│   │   └── Should_*() tests              ← Assertions (Then)
│   └── But_condition_2 : When_scenario_A  ← Alternative condition
└── When_scenario_B : RootSpecs
```

Rules:
- **One When() per functionality group** — single-method classes: one `When()` at root; multi-method classes: one `When_` group per method at level 2, each with its own `When()`. Children inherit `When()` — do NOT override (except `Catch.Exception` wrapping). **Placement**: define `When()` at the **highest level** where all children share the same action — children vary context via `Given()`, not by duplicating `When()` at each level
- **`And_`/`But_` are level 2+ only** — root children always use `When_` prefix
- **Multi-method vs configuration specs** — multiple independent methods → flat `When_` siblings. Interacting parameters within an operation → nest. **Fluent APIs, Builders, and Chain-of-Responsibility are ALWAYS configuration-style** — each method configures a parameter, not an independent operation. See [unit-configuration-builder](examples/unit-configuration-builder.md) for WRONG vs CORRECT
- **Given() is additive** — parent→child order guaranteed by reflection. Do NOT call `base.Given()` — framework invokes each level separately; calling it causes double execution
- **Test methods are pure assertions** — Given/When already ran at construction time

### Deliberate Discovery (Pre-Step)

Before listing parameters, surface what you **don't know** (Keogh). Apply three questions to the production class:

| Pattern | Question | Reveals |
|---------|----------|---------|
| **Context** | "What context or precondition would change the outcome?" | Hidden branches, domain boundaries |
| **Outcome** | "What outcomes could happen (including failures)?" | Success + failure paths |
| **Interaction** | "How does the actor interact with the system?" | Workflow, sequencing constraints |

These questions produce the **same parameters** as analytic enumeration but from a discovery perspective — they surface assumptions that static analysis misses.

**Break the Model**: Actively seek examples that **contradict** your mental model. An example that surprises you reveals a hidden assumption. Celebrate these — they prevent production bugs.

**Implied Concept Detection** (Adzic): When you need >5 scenarios for a single rule, a **hidden domain concept** exists that should be named and extracted. In practice: if the combinatorial tree reaches 6+ levels, look for a concept to extract as its own class with its own spec.

### Scenario Discovery for Full Coverage

Discover scenarios systematically:

1. **List parameters** the action depends on
2. **Enumerate possible states** for each parameter
3. **Identify determinant parameters** — those whose state affects expected behavior
4. **Group related determinant parameters** that interact
5. **Build combinatorial hierarchy** — one dimension per nesting level, determinant parameters only
6. **Non-determinant parameters**: use any valid value; don't mention in context/spec names
7. **Unrelated concerns** → separate scenario trees
8. **Deduplicate** — build from **present/active** states. Absent states appear as `But_` leaves. All-absent is a standalone context. Do NOT create absent-first branches — they duplicate present-first coverage
8b. **Group parameters** when list is long — each group becomes one nesting dimension. Nesting by group (3 levels) is manageable; by individual parameter (10+) is not

Don't fear deep nesting (3-5 levels) when driven by combinatorial coverage.

### Coverage Strategy by Spec Level

Coverage depth varies by level — exhaustive everywhere is neither practical nor valuable:

| Level | Strategy | Rationale |
|-------|----------|-----------|
| **Unit** | Exhaustive combinatorial | Fast, cheap, domain is known |
| **Service** | Exhaustive for determinants; key examples for edge cases | Balance coverage and cost |
| **Module** | Key examples: happy path + 1-2 failure paths per collaborator | Mocks make exhaustive combinations fragile |
| **Application** | Key examples only: happy path + critical error paths | Infrastructure-heavy; each scenario is expensive |

"Key examples" (Adzic): a small focused set that **clarifies behavior** beats exhaustive enumeration for understanding. When too many examples are needed, find the implied concept (§2 Deliberate Discovery).

### Stable-First Nesting Principle

Order contexts from least-varying (outermost) to most-varying (innermost):
1. Parameter whose states partition the most scenarios → outermost
2. Equal partitioning → prefer categorical/structural outermost, data/value innermost
3. Significant parameter ordering (fluent API call order) → treat as a test dimension

### Large Scope Decomposition

When covering a large API surface: **decompose into independent feature areas first**, then apply Scenario Discovery per area independently. Do NOT plan all scenarios across all features simultaneously — this causes breadth-over-depth degradation.

1. Partition into independent feature groups
2. For EACH group: full Scenario Discovery workflow
3. Write one spec file at a time with full depth before moving to next
4. Cross-group interactions get their own end-to-end specs

### Deferred Scenarios (Real Options)

When the scenario tree produces more scenarios than worth writing now, **defer as pending tests** — not as TODO comments. Pending scenarios are **real options** (Matts): they have value, they may expire, and you exercise them when risk justifies it.

Create the class structure with `[Ignore]` and empty body — the scenario is visible in the test hierarchy, compiles (refactoring catches renames), and appears as "Skipped" in test results:

```csharp
[TestFixture]
[Ignore("Deferred: VIP tier scenarios — exercise when VIP pricing is confirmed")]
public class When_the_member_is_VIP : FineCalculatorSpecs
{
    // Given()/When()/Should_ stubs added when option is exercised
}
```

**When to defer**: scenario is combinatorially valid but low-risk, awaiting business confirmation, or outside the current delivery scope. **When to exercise**: risk increases, business confirms the rule, or you need the coverage.

### Mandatory Pre-Generation Gate

**STOP** — before writing any spec code, output and validate the Scenario Discovery tree:

1. **Parameter inventory** — every parameter, states, determinant/non-determinant
2. **Scenario tree** — full hierarchy with all `When_`/`And_`/`But_` class names (mark deferred scenarios with `[deferred]`)
3. **Leaf count** — total leaf scenarios (implemented + deferred)
4. **Deduplication check** — no absent-first duplicates

**Self-check** (all must be YES):
- Every determinant parameter appears as a nesting dimension?
- `And_`/`But_` children under `When_` groups (not just flat siblings)?
- Context names answer "what condition exists?" (not "what happens")?
- For builder/fluent APIs: parameters nested, not compounded?
- Leaf count ≥ product of determinant parameter states?

## 3. Write the Specification

### Setup and Imports

Adapt imports, test attributes, and assertion syntax to the detected stack (§0). Root class inherits `FeatureSpecifications`; all shared fields are `protected`.

### When() as Single SUT Invocation

`When()` must be **expression-bodied** — one invocation of the SUT's public method, capturing the result. All setup belongs in `Given()`. Only exception: genuinely multi-statement SUT (rare, must be justified).

Result naming: `_result` (generic) or domain-specific (`_response`, `_exception`).

### Naming Conventions

See [naming conventions](references/naming-conventions.md) for complete rules. Key principles:
- **Scenario as state transition**: Given = precondition state, When = transition trigger, Then = new state. Each scenario specifies a **behaviour change**, not a capability
- Context names express **domain state** (`When_the_member_is_Standard`), not consequence
- Test methods express **expected outcome** (`Should_apply_a_25_percent_discount`)
- Lambda formatting: always break after `=>`

### Exception Specification

Use `Catch.Exception()` in `When()` — never try/catch. Store in `_exception` field. For `Task`-returning async operations, use the `Func<Task>` overload: `Catch.Exception(async () => await _service.DoAsync())`. Use the generic `Catch.Exception<TException>()` overloads to get the exception already cast, avoiding manual casts in assertions. See [exception-specification](examples/exception-specification.md).

### Async Given()/When()

Use `async void` when SUT or dependencies are async. The framework tracks async continuations and blocks until each step completes before proceeding to the next. Execution order is preserved: parent `Given()` → child `Given()` → `When()`.

```csharp
public override async void Given() =>
    _server = await CreateTestServerAsync();

public override async void When() =>
    _response = await _client.GetAsync(_requestPath);
```

### Mocking (Service/Module Level)

Use the project's detected mocking library (§0). Base `Given()` creates all mocks at happy-path; children override single stubs. See [module-coordinator](examples/module-coordinator.md).

### Polymorphic Return Specification

Add cast helper properties on the base class (`ResultAsApproval`, `ResultAsRejection`). Type assertion first. See [service-factory-polymorphic](examples/service-factory-polymorphic.md).

### Post-Generation Self-Check

After writing any spec code, verify these structural rules before reporting completion. These catch the most common generation errors:

1. **No duplicate `When()` bodies** — grep for `override void When()` or `override async void When()` in the file. If two or more have the same expression body, move `When()` to their common ancestor
2. **Every spec class inherits `FeatureSpecifications`** — no plain NUnit classes, no `[OneTimeSetUp]`, no constructor-based setup
3. **No `[TearDown]`** — BDDExtensions creates separate instances per fixture; shared-state cleanup is unnecessary
4. **Context names are domain state** — re-read each `When_`/`And_`/`But_` name and ask: "does this describe what `Given()` configures, or what `When()` produces?" If the latter, rename

## 4. Fractal Spec Composition

The same Context/Specification structure repeats at every test pyramid level — a **fractal** pattern where each level decomposes the outcomes of the level above.

### Fractal Chain: THEN@N = GIVEN@N+1

The outcome asserted at one level becomes the precondition at the next level down. This is the mechanism that **connects** spec files across pyramid levels:

```
Application (LibraryAPISpecs):
  When_borrowing_a_book / And_the_book_is_available
    → Should_produce_a_loan_with_correct_due_date           ← THEN

Module (LendingServiceSpecs):
  When_a_loan_is_being_produced                              ← mirrors THEN above
    → Should_delegate_to_factory_with_member_tier            ← THEN

Service (LoanRequestFactorySpecs):
  When_the_member_is_eligible / And_the_member_is_Standard   ← mirrors THEN above
    → Should_set_the_loan_period_to_14_days                  ← THEN
```

Each `Should_` at level N names the **outcome**. The `When_` at level N+1 names that **same outcome as a context/action**. The naming need not be identical, but the **domain concept must be traceable** between levels.

### Fractal Naming Correspondence

Apply naming deliberately to make the fractal chain readable across spec files:

| Level N (THEN) | Level N+1 (GIVEN/WHEN context) |
|----------------|-------------------------------|
| `Should_produce_a_loan_with_correct_due_date` | `When_a_loan_is_being_produced` |
| `Should_delegate_to_factory_with_member_tier` | `When_the_member_is_eligible` |
| `Should_calculate_due_date_from_tier_loan_period` | `When_a_discount_is_applied` → Given: member tier |

**Rule**: When writing a `Should_` method that describes behavior delegated to a lower level, check whether a spec exists (or should exist) at that level with a corresponding `When_` context. If not, create it — or create a [deferred scenario](#deferred-scenarios-real-options) as a pending test.

### Outside-In Writing Order

Write specs **top-down from value** (Matts/Keogh/Adzic convergence):

1. **Application** spec first — what the stakeholder sees/needs
2. **Module** specs — coordination, side effects, error routing
3. **Service** specs — conditional logic, polymorphic returns
4. **Unit** specs — exhaustive determinant parameter combinations

Each level's `Should_` methods reveal what the next level down must specify. This is not a rigid sequence — start from whichever level the current task targets — but when building a new feature top-down, follow value→implementation order.

### Delegation Principle

Each level stubs the level below at happy path and focuses scenarios on its own responsibility:

| Level | Specs Focus | Level Below |
|-------|-------------|-------------|
| **Orchestrator** | Coordination, side effects, error routing | Factory/Service mocked at happy-path |
| **Service** | Conditional logic, polymorphic returns | Evaluator/Calculator at happy-path |
| **Unit** | Exhaustive determinant parameter combinations | No delegation |

Edge cases at each level are for **that level's responsibility**, not delegated work below.

### Application/Module Considerations

- Root `Given()` creates ALL infrastructure (test server, mocks, seed data, auth context)
- Child `Given()` overrides ONE condition
- See [application-api-endpoint](examples/application-api-endpoint.md)

### Cleanup

When root `Given()` creates disposable infrastructure (test servers, database connections), override `Cleanup()` **at the bottom of the root spec class**, after all scenario groups and helper methods. The framework calls it automatically via `IDisposable` after all tests in the fixture complete:

```csharp
[TestFixture]
public class MyApiSpecs : FeatureSpecifications
{
    protected ApiTestServer _server;

    public override void Given() { _server = new ApiTestServer(...); }
    public override void When() { /* ... */ }

    public class When_scenario_A : MyApiSpecs { /* ... */ }
    public class When_scenario_B : MyApiSpecs { /* ... */ }

    public override void Cleanup() =>
        _server.DisposeAsync().GetAwaiter().GetResult();
}
```

**Placement convention**: `Given()` at the top (setup), `Cleanup()` at the bottom (teardown) — mirrors the lifecycle order.

### Test Infrastructure via Composition

Spec classes inherit **only** from `FeatureSpecifications` — never from project-specific base classes. Extract infrastructure into a composition-based context class created in root `Given()`. Infrastructure is a collaborator, not a parent.

## 5. Validate

After writing a spec, run `/simplify` on the new code, then verify:
- [ ] 1:1 mapping: spec file name matches production class
- [ ] Single When() per functionality group, defined at the highest shared level
- [ ] `Cleanup()` override at the bottom of root class if disposable infrastructure exists
- [ ] Hierarchy reads as natural language (living documentation)
- [ ] No duplicate setup across levels (each Given() adds, not repeats)
- [ ] Fields are `protected`
- [ ] Expression-bodied members for single-line implementations
- [ ] Assertions use the detected assertion library consistently (§0)
- [ ] Combinatorial coverage appropriate for spec level (§2 Coverage Strategy)
- [ ] Context names express domain state, not consequence
- [ ] Lambda formatting: `=>` followed by newline
- [ ] Non-determinant parameters not included in combinations
- [ ] `When()` is a single expression-bodied SUT invocation
- [ ] Spec inherits only from `FeatureSpecifications`
- [ ] No equivalent duplicate scenarios (absent-first branches pruned)
- [ ] Deferred scenarios use `[Ignore]` with reason, not TODO comments
- [ ] Fractal naming: `Should_` at level N traceable to `When_` at level N+1

## Examples

Examples use NUnit + Shouldly + NSubstitute as concrete illustrations. Adapt to detected stack (§0).

- [Unit: Value Object](examples/unit-value-object.md) — ISBN validation with Catch.Exception
- [Unit: Deep Nesting](examples/unit-deep-nesting.md) — Fine comparisons at 4-5 levels
- [Unit: Calculator](examples/unit-calculator.md) — FineCalculator with tier combinations
- [Unit: Configuration Builder](examples/unit-configuration-builder.md) — WRONG (flat) vs CORRECT (nested)
- [Service: Evaluator](examples/service-evaluator.md) — Stateful LoanEligibilityEvaluator
- [Service: Polymorphic Factory](examples/service-factory-polymorphic.md) — LoanApproval vs LoanRejection
- [Module: Coordinator](examples/module-coordinator.md) — LendingService with mocks
- [State Machine](examples/state-machine.md) — BookLifecycle transitions
- [Application: API](examples/application-api-endpoint.md) — HTTP endpoint
- [Exception Patterns](examples/exception-specification.md) — Comprehensive Catch usage
- [Fractal Chain](examples/fractal-chain.md) — THEN@N=GIVEN@N+1 naming + deferred scenarios

## References

- [Naming Conventions](references/naming-conventions.md) — Complete naming rules
- [Pattern Catalog](references/pattern-catalog.md) — All spec patterns categorized
- [Anti-Patterns](references/anti-patterns.md) — Common mistakes and fixes

## Prompts

- [Create Specification](prompts/create-spec-context.md) — All levels
- [Add Context Level](prompts/add-context-level.md) — Extend existing spec
- [Review Spec](prompts/review-spec.md) — Validate and improve existing spec
- [Plan Coverage](prompts/plan-coverage.md) — Pre-writing coverage blueprint

## Cross-Skill Dependencies

- Reads CLAUDE.md for testing stack detection (§0)
- Uses `/ai-summaries` format for all generated .md artifacts
- Uses `/bdd-consulting` for BDD methodology: Deliberate Discovery (Keogh), Feature Injection & Real Options (Matts), Specification by Example & Impact Mapping (Adzic)

## Self-Improvement

On every execution, this skill evaluates:
- Uncovered scenarios or patterns not in the knowledge base
- Weaknesses in recommendations provided
- New insights from sources being processed

When detected → user is offered:
- Apply immediately
- Add to ROADMAP.md (created if missing)
