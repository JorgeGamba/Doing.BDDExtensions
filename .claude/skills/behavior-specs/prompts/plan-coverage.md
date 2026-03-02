# Prompt: Plan Scenario Coverage

**Usage:** Generate the coverage blueprint before writing any spec.
**Customize:** Replace `{ProductionClass}` with the class name.

---

## Prompt

Use `/behavior-specs` to plan scenario coverage for `{ProductionClass}`.

1. **Assess scope** — large API? Decompose into independent feature areas; apply steps 2-7 per area
2. **Read the production class** — list public methods and signatures
3. **List parameters/inputs** per operation — constructor args, method params, object state, collaborator returns
4. **Enumerate states** per parameter — e.g., tier: Standard/Premium/VIP
5. **Identify determinant parameters** — only these drive combinatorial coverage
6. **Build combinatorial tree** — one dimension per nesting level; Stable-First ordering
7. **Deduplicate** — prune absent-first branches; all-absent is standalone

Output format:
```
{ProductionClass}Specs : FeatureSpecifications
├── Given() → [shared setup]
├── When() → [action]
├── When_{group_1}
│   ├── And_{state_A} → [leaf]
│   └── But_{error} → [Catch.Exception]
└── When_{group_2} → ...

Non-determinant: paramX (use: "default_value")
Leaf count: N
```

---

## Notes for Claude

- Read SKILL.md §2 (Scenario Discovery) before starting
- Self-check (all YES before writing code): determinant parameters as dimensions? `And_`/`But_` children? Domain-state names? Leaf count ≥ product of states? Absent-first pruned?
