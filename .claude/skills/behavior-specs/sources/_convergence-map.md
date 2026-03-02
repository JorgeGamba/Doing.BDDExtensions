# Convergence Map: behavior-specs

> 14 ideas tracked | 1 source | Last updated: 2026-04-04

## ★★★ Invariant (weight ≥ 3)

| Idea | Weight | Sources | Origin |
|------|--------|---------|--------|

## ★★ Strong (weight = 2)

| Idea | Weight | Sources | Notes |
|------|--------|---------|-------|

## ★ Single (weight = 1)

| Idea | Weight | Sources | Notes |
|------|--------|---------|-------|
| Characterisation test as first spec for walking skeleton — establish coverage before TDD | 1 | seemann-ctfiyh | Ch 4 |
| AAA pattern: "rotate 90°, balance on Act" formatting heuristic; blank lines as phase delimiters | 1 | seemann-ctfiyh | Ch 4 |
| Outside-in TDD: start at HTTP boundary, work inward; boundary tests alone cause combinatorial explosion | 1 | seemann-ctfiyh | Ch 4 |
| Value Objects enable structural-equality assertions in tests | 1 | seemann-ctfiyh | Ch 4 |
| Tests as external memory for behavior — alternative to memorising legacy code structure | 1 | seemann-ctfiyh | Ch 6 |
| Devil's Advocate technique: write obviously wrong implementation that passes; if you can, need more tests | 1 | seemann-ctfiyh | Ch 6 |
| Devil's Advocate vs RGR: complementary — DA drives test case addition, RGR drives code generalisation | 1 | seemann-ctfiyh | Ch 6 |
| Test sufficiency as risk assessment: probability × impact, no formula | 1 | seemann-ctfiyh | Ch 6 |
| No safety net for test code — safest edits are additions (new tests, cases, assertions) | 1 | seemann-ctfiyh | Ch 11 |
| Adding assertions strengthens postconditions (LSP analogy) — strengthening OK, weakening not | 1 | seemann-ctfiyh | Ch 11 |
| Separate refactoring of test and production code — commit independently; use git stash to enforce | 1 | seemann-ctfiyh | Ch 11 |
| Always see a test fail before trusting it — sabotage SUT temporarily if needed | 1 | seemann-ctfiyh | Ch 11 |
| Property-based testing: replace hard-coded irrelevant values with generated inputs; for complex logic where enumeration is harder than invariant description | 1 | seemann-ctfiyh | Ch 15 |
| Behavioural code analysis: Git-derived hotspots (change frequency × complexity), change coupling maps, knowledge maps — test prioritisation signals | 1 | seemann-ctfiyh | Ch 15 |

## Source Registry

| Source | Authority | Topics Extracted | Status |
|--------|----------|-----------------|--------|
| seemann-ctfiyh | 1-Normal | 4 | complete |

## Cross-Skill Resonance

| Skill | Resonant Ideas | Notes |
|-------|---------------|-------|
| code-design | AAA, Devil's Advocate, test refactoring, property-based testing | Same ideas extracted in code-design with different lens |
| functional-programming | Pure function tests survive refactoring | Property-based testing relates to FP testing patterns |
