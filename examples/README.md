# Examples

This folder contains illustrative production code and BDD specifications using the **Library System** domain to demonstrate all features and patterns of `Doing.BDDExtensions`.

## Structure

```
examples/
├── Library.Domain/          # Production code (the system being specified)
│   ├── ISBN.cs              # Value object with format validation
│   ├── Fine.cs              # Value object with tolerance-based equality
│   ├── FineCalculator.cs    # Calculator with tier/type multipliers
│   ├── LoanEligibilityEvaluator.cs  # Multi-condition evaluator
│   ├── LoanRequestFactory.cs        # Factory returning LoanApproval or LoanRejection
│   ├── LendingService.cs            # Module orchestrating collaborators
│   ├── BookLifecycle.cs             # State machine (Available → OnLoan → Overdue → ...)
│   ├── ILoanRequest.cs              # Polymorphic result types
│   ├── IBookRepository.cs           # Repository interfaces
│   ├── Member.cs                    # Entity
│   ├── MembershipTier.cs            # Enum
│   ├── ItemType.cs                  # Enum
│   └── BookState.cs                 # Enum
│
└── Library.Specs/           # BDD Specifications (1:1 mapping to production)
    ├── ISBNSpecs.cs                 # Pattern: value object validation + Catch.Exception
    ├── FineSpecs.cs                 # Pattern: deep nesting (4+ levels), tolerance comparison
    ├── FineCalculatorSpecs.cs       # Pattern: calculator with dimension combinations
    ├── LoanEligibilityEvaluatorSpecs.cs  # Pattern: stateful evaluator, condition paths
    ├── LoanRequestFactorySpecs.cs        # Pattern: polymorphic return types
    ├── LendingServiceSpecs.cs            # Pattern: module orchestration with mocks
    └── BookLifecycleSpecs.cs             # Pattern: state machine transitions
```

## Patterns Demonstrated

| Spec File | Level | Pattern | Hierarchy Depth |
|-----------|-------|---------|-----------------|
| ISBNSpecs | Unit | Value object validation + Catch.Exception | 2 |
| FineSpecs | Unit | Deep nesting, tolerance comparison, And_/But_ | 4-5 |
| FineCalculatorSpecs | Unit | Calculator with tier × type combinations | 3-4 |
| LoanEligibilityEvaluatorSpecs | Service | Stateful evaluator, condition paths | 3 |
| LoanRequestFactorySpecs | Service | Polymorphic returns (Approval vs Rejection) | 3 |
| LendingServiceSpecs | Module | Orchestration with NSubstitute mocks | 2-3 |
| BookLifecycleSpecs | Module | State machine transitions + exception specs | 3-4 |

## Fractal Delegation Chain

The 7 spec files form a **test pyramid** where each level stubs/mocks the level below at happy-path and focuses its own scenarios on its own responsibility:

```
                    ┌─────────────────────────────┐
                    │     LendingServiceSpecs      │  Module: mocks Factory +
                    │   (orchestrator, 2-3 deep)   │  Repositories at happy-path
                    └──────────────┬──────────────┘
                                   │
                    ┌──────────────┴──────────────┐
                    │  LoanRequestFactorySpecs     │  Service: uses real Evaluator
                    │   (polymorphic, 3 deep)      │  at happy-path
                    └──────────────┬──────────────┘
                                   │
              ┌────────────────────┴────────────────────┐
              │                                         │
   ┌──────────┴──────────┐               ┌─────────────┴─────────────┐
   │ LoanEligibility-    │               │    FineCalculatorSpecs     │
   │ EvaluatorSpecs      │               │   (tier×type combos,      │
   │ (conditions, 3 deep)│               │    3-4 deep)              │
   └─────────────────────┘               └───────────────────────────┘
                                                      │
                                         ┌────────────┴────────────┐
                                         │                         │
                                   ┌─────┴─────┐           ┌──────┴──────┐
                                   │ FineSpecs  │           │  ISBNSpecs  │
                                   │(4-5 deep)  │           │ (2 deep)    │
                                   └────────────┘           └─────────────┘
```

**How the chain works:**
- **LendingServiceSpecs** (top) — mocks `IBookRepository`, `ILoanRecordRepository`, `IFineRepository` at happy-path; tests coordination and side-effects
- **LoanRequestFactorySpecs** — uses real `LoanEligibilityEvaluator` at happy-path; tests polymorphic returns (Approval vs Rejection)
- **LoanEligibilityEvaluatorSpecs** — pure logic, no delegation; tests condition paths (suspension, fines, loan limits)
- **FineCalculatorSpecs** — exhaustive tier×itemType combinations; no delegation
- **FineSpecs**, **ISBNSpecs** — value object validation; deepest unit specs

Each level only tests its **own** logic. Happy-path of the level below is assumed correct — its edge cases belong to that level's own spec file. `BookLifecycleSpecs` sits alongside as an independent state-machine spec.

## How to Explore

Start with `ISBNSpecs.cs` for the simplest pattern, then progress through the table above for increasing complexity. Each spec file includes a summary comment at the top describing which patterns it demonstrates.

> **Note**: These examples are illustrative code meant to demonstrate BDD specification patterns. They are not a compilable standalone project. To see compilable specs that exercise the framework, see the [test project](../src/Doing.BDDExtensions.Specs/).
