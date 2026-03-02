# Doing.BDDExtensions

C# library (.NET Standard 2.0) — BDD-style Given-When-Then test infrastructure for NUnit.

## CLAUDE.md Maintenance

- Keep <300 lines; only universally applicable content
- `file:line` pointers over code snippets
- Procedural workflows → skills (not here)

## Git Restrictions

Only run read-only git commands unless the user explicitly requests a modifying operation.

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

Two source files in core library:
- `FeatureSpecifications.cs` — abstract base class; Template Method pattern with reflection-based hierarchical Given→When execution
- `Catch.cs` — static utility to capture exceptions for assertion

## Key Patterns

- **Template Method** — `FeatureSpecifications` base class with virtual `Given()` / `When()`
- **Inheritance-based context nesting** — child classes override `Given()` at each level; base Givens execute parent→child order via reflection
- **BDD test naming** — test classes: `When_<scenario>`, test methods: `Should_<expectation>`

## Critical Constraints

- **Main library MUST have zero NuGet dependencies** — only System and System.Reflection
- **Given execution order is parent→child** — `InvokeBaseGivenIfExists` walks the type hierarchy recursively; do not break this ordering
- **Constructor triggers step execution** — `ThrowSteps()` is called in `FeatureSpecifications` constructor; Given/When run at construction time

## Naming Conventions

| Type | Pattern | Example |
|---|---|---|
| Private fields | `_camelCase` | `_order`, `_exception` |
| Test classes | `When_<scenario>` | `When_the_Given_and_When_parts_are_both_implemented` |
| Test methods | `Should_<expectation>` | `Should_invoke_the_Given_first_and_then_the_When` |

## Code Conventions

- Expression-bodied members for single-line implementations
- One public class per file
- Shouldly for fluent assertions in specs
- NUnit `[OneTimeSetUp]` for spec setup (not constructor)
