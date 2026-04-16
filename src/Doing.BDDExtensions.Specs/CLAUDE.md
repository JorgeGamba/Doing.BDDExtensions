<!-- Loaded when working in the Specs project. See root CLAUDE.md for project-wide rules. -->

# Specs Project

Specifications for the Doing.BDDExtensions core library. Uses NUnit 4 + Shouldly.

## Writing Specs

**Dogfooding** — this project tests itself with itself. All spec classes MUST inherit from `FeatureSpecifications` and use the standard GWT pattern: override `Given()`/`When()`, nest with `When_`/`And_`/`But_`. Use `/behavior-specs` for creating and maintaining specs.

## Test Stack

- **Runner:** NUnit 4
- **Assertions:** Shouldly
- **BDD base:** `FeatureSpecifications`
