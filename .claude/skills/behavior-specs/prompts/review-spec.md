# Prompt: Review Specification

**Usage:** Review an existing spec for correctness, completeness, and convention adherence.
**Customize:** Replace `{specFile}` with the path to the spec file.

---

## Prompt

Use `/behavior-specs` to review `{specFile}`.

### Structure
- [ ] Single `When()` per functionality group (children don't override except `Catch.Exception`)
- [ ] Hierarchy depth 2-5 levels (not 6+)
- [ ] Class names read as natural language
- [ ] File name matches production class: `{ProductionClass}Specs.cs`
- [ ] `When()` is a single expression-bodied SUT invocation
- [ ] Spec inherits only from `FeatureSpecifications`
- [ ] No duplicate scenarios (absent-first branches pruned)
- [ ] Stable-First nesting order

### Naming
- [ ] Root: `{ProductionClass}Specs`; scenarios: `When_`; conditions: `And_`/`But_`; tests: `Should_`
- [ ] Context names express domain state, not consequence
- [ ] Lambda formatting: `=>` followed by newline

### Setup
- [ ] Root `Given()` creates SUT and defaults; children only set delta
- [ ] No setup in constructors; exception paths use `Catch.Exception()`
- [ ] Fields are `protected`

### Assertions
- [ ] One assertion per test method
- [ ] Consistent assertion library
- [ ] Tests behavior, not implementation

### Completeness
- [ ] Happy path, error paths, boundary conditions covered
- [ ] Combinatorial coverage: determinant parameters composed hierarchically
- [ ] Non-determinant parameters excluded from combinations

---

## Notes for Claude

- Read the production class to understand expected behavior before reviewing
- Flag: multiple `When()` overrides, private fields, generic names, repeated Given() setup
- Flag: consequence-based context naming, flat scenarios missing combinations
- Check against [anti-patterns](../references/anti-patterns.md)
