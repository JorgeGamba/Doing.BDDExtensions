# Prompt: Create Specification

**Usage:** Create a BDD spec for a production class at any level (unit, service, module, application).
**Customize:** Replace `{ProductionClass}` with the class name, `{specLevel}` with unit/service/module/application.

---

## Prompt

Use `/behavior-specs` to create a specification for `{ProductionClass}` at `{specLevel}` level.

1. **Detect testing stack** — identify test runner, assertion library, and mocking library from CLAUDE.md, `.csproj`, or existing specs (SKILL.md §0)
2. **Assess scope** — if multiple features or large API, decompose into independent areas first; apply steps 3-7 per area
3. **Read the production class** — identify public methods/responsibilities; single-method → one root `When()`; multi-method → one `When_` group per method
4. **Discover scenarios** — list parameters, enumerate states, identify determinants, build combinatorial hierarchy (SKILL.md §2)
5. **Validate the scenario tree** — output tree and verify: determinant parameters as nesting dimensions, `And_`/`But_` children under `When_` groups, domain-state names, leaf count ≥ product of determinant states
6. **Write the spec** — follow [naming conventions](../references/naming-conventions.md); `protected` fields, `Catch.Exception()` for errors, expression-bodied `When()`, lambda break after `=>`
7. **Validate** — check against [anti-patterns](../references/anti-patterns.md)

---

## Notes for Claude

- Read SKILL.md §0-§5 before generating any code
- Always output the scenario tree for user validation BEFORE writing code (Mandatory Pre-Generation Gate)
- Apply Stable-First Principle: most stable dimension outermost
- Spec inherits only from `FeatureSpecifications` — use composition for infrastructure
- `When()` must be a single expression-bodied SUT invocation
- For builder/fluent APIs: parameters are always configuration-style — nest, don't flatten
