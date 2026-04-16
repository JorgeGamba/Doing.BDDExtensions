# Doing.BDDExtensions

C# library (.NET Standard 2.0) — BDD-style Given-When-Then test infrastructure for NUnit.

## CLAUDE.md Self-Protection

- Keep <300 lines; only universally applicable content
- `file:line` pointers over code snippets; ≤10-line snippet only for churny targets
- Procedural workflows → skills (not here)
- Format/style rules → `.editorconfig` (not here)

## Git Restrictions

Only run read-only git commands unless the user explicitly requests a modifying operation.

## Documentation Standards

- Human docs (README, docs/): plain English — for humans
- AI artifacts (skills, references, CLAUDE.md): apply `/ai-summaries`

## Build & Test

- **Solution:** `src/Doing.BDDExtensions.sln`
- **Build:** `dotnet build src/Doing.BDDExtensions.sln`
- **Test:** `dotnet test src/Doing.BDDExtensions.Specs/Doing.BDDExtensions.Specs.csproj`
- **Pack:** `dotnet pack src/Doing.BDDExtensions/Doing.BDDExtensions.csproj -c Release`

## Architecture

| Project | Target | Role |
|---|---|---|
| `Doing.BDDExtensions` | netstandard2.0 | Core library — zero dependencies |
| `Doing.BDDExtensions.Specs` | net8.0 | NUnit 4 specs + Shouldly assertions |

Three source files in core library:
- `FeatureSpecifications.cs` — abstract base class; Template Method pattern with reflection-based hierarchical Given→When execution
- `AsyncRunner.cs` — internal utility; tracks `async void` continuations via custom `SynchronizationContext`, blocks until complete
- `Catch.cs` — static utility to capture exceptions for assertion

## Key Patterns

- **Template Method** — `FeatureSpecifications` base class with virtual `Given()` / `When()`
- **Inheritance-based context nesting** — child classes override `Given()` at each level; base Givens execute parent→child order via reflection
- **BDD test naming** — test classes: `When_<scenario>`, test methods: `Should_<expectation>`
- **Writing BDD specifications** → see skill: `behavior-specs`

## Critical Constraints

- **Main library MUST have zero NuGet dependencies** — only System and System.Reflection
- **Given execution order is parent→child** — `InvokeBaseGivenIfExists` walks the type hierarchy recursively; do not break this ordering
- **Constructor triggers step execution** — `ThrowSteps()` is called in `FeatureSpecifications` constructor; Given/When run at construction time
- **Async void is supported transparently** — `AsyncRunner.Run()` wraps each `Given()`/`When()` invocation; `async void` overrides complete before the next step begins

## Naming Conventions

| Type | Pattern | Example |
|---|---|---|
| Test classes | `When_<scenario>` | `When_the_Given_and_When_parts_are_both_implemented` |
| Test methods | `Should_<expectation>` | `Should_invoke_the_Given_first_and_then_the_When` |

## Code Conventions

- Naming and formatting rules are enforced by `.editorconfig` — read it before writing C# code; do not duplicate those rules here
- Expression-bodied members for single-line implementations
- One public class per file
- Shouldly for fluent assertions in specs
- NUnit `[OneTimeSetUp]` for spec setup (not constructor)

### Comments
- Comments describe **WHAT/WHY**, never **HOW** — explain behavior and rationale
- **NEVER noise comments** — `// increments counter` is noise; delete it
- Prefer code over comment — extract complex logic to a named method or variable
- `///` XML comments for all public APIs; skip only for trivially self-evident members (e.g., `Name { get; }`, obvious setters)

### Clean Code Style
- **Name to reveal intent, concretely** — method name states what it does without reading its body
- **One name per concept** — don't mix `fetch`/`retrieve`/`get` for the same operation
- **Distinguish meaningfully** — names must differ in intent, not decoration
- **Avoid disinformation** — `accountList` MUST be a `List<>`
- **Behavior matches name** — `getX` MUST NOT modify state
- **Do One Thing at one level of abstraction**
- **Keep methods short** — extract when a method exceeds ~15 lines
- **Stepdown Rule** — caller above callee; private helpers at the bottom
- **DRY** — extract when the same logic appears twice
- **Orthogonality** — changing X must not require changing Y
- **No pass-through methods** — a method that only delegates without adding value must not exist
- **Introduce explaining variables** — name complex expressions
- **Decompose conditionals** — extract complex conditions to named boolean methods
- **Define errors out of existence** — design so invalid states are impossible rather than handled
