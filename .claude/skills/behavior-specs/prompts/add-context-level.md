# Prompt: Add Context Level

**Usage:** Add a new scenario, condition variation, or edge case to an existing spec file.
**Customize:** Replace `{specFile}` with the path, `{newContext}` with a description of the scenario to add.

---

## Prompt

Use `/behavior-specs` to add a context level to `{specFile}` for: `{newContext}`.

1. **Read the existing spec** — understand current hierarchy and coverage
2. **Identify where new context fits**:
   - New scenario group? → sibling `When_*` class
   - New condition on existing scenario? → child `And_*` or `But_*`
   - New edge case? → child of most specific matching context
3. **Determine Given() needs** — only override what's different from parent; add `protected` field to nearest ancestor if needed
4. **Write the new context** — name after domain state (not outcome); expression-bodied for single-line Given/Should
5. **Validate**:
   - Hierarchy reads naturally?
   - `Given()` is minimal (only delta from parent)?
   - No duplicate setup with siblings?
   - Does this complete a combinatorial group? Add sibling contexts for other states?
   - Lambda formatting: break after `=>`?
   - Stable-First ordering preserved?

---

## Notes for Claude

- Read the full existing spec before making changes
- Context names express domain state, not consequence
- Check if adding this context reveals missing sibling combinations
- Parent `When()` remains a single expression-bodied SUT invocation — do not modify it
