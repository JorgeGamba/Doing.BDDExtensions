# Pattern Catalog

> Patterns extracted from Doing.BDDExtensions usage across 3 real-world projects (60+ spec files)

## Unit-Level Patterns

### Default/Uninitialized State
- **When**: Type with meaningful default state (default struct, unset value object)
- **Structure**: Empty `When()` override; 2 levels
- **Key**: Assert default state explicitly (`ISBNSpecs`)

### Value Object Validation
- **When**: Constructor/factory with validation rules
- **Structure**: Root `When()` wraps `Catch.Exception()`; children set inputs via `Given()`; 2 levels
- **Key**: Each child tests one input variation; see [unit-value-object](../examples/unit-value-object.md)

### Deep Comparison/Arithmetic
- **When**: Value objects with equality, comparison, or arithmetic operators
- **Structure**: Multiple operation groups at level 2; condition variations at level 3-4; 4-5 levels
- **Key**: Test tolerance boundaries; see [unit-deep-nesting](../examples/unit-deep-nesting.md)

### Calculator with Combinations
- **When**: Pure functions with multiple input dimensions (tier × item type × duration)
- **Structure**: One dimension per nesting level; exhaustive combinations at leaf; 3-4 levels
- **Key**: Include cap/boundary tests; see [unit-calculator](../examples/unit-calculator.md)

## Service-Level Patterns

### Stateful Evaluator
- **When**: Service evaluates against thresholds or accumulates state
- **Structure**: Root `Given()` sets happy path; children deviate one condition; 3 levels
- **Key**: No mocks needed — pure logic; see [service-evaluator](../examples/service-evaluator.md)

### Polymorphic Return Factory
- **When**: Factory returns different concrete types based on conditions
- **Structure**: Root `When()` calls factory; cast helpers on base class; 3 levels
- **Key**: Type assertion first, then property assertions; see [service-factory-polymorphic](../examples/service-factory-polymorphic.md)

### Sequential/Stateful Factory
- **When**: Factory maintains state across calls; each call builds on previous
- **Structure**: Base `Given()` calls factory N times; `When()` makes final call; 2-3 levels
- **Key**: Child `Given()` adds calls to build desired state

## Module-Level Patterns

### Orchestrator with Mocks
- **When**: Service coordinates multiple collaborators
- **Structure**: Root `Given()` creates all mocks (happy path); children override one stub; 2-3 levels
- **Key**: Test return values AND interactions; see [module-coordinator](../examples/module-coordinator.md)

### State Machine Transitions
- **When**: Object with state transitions and validity rules
- **Structure**: One `When_` per transition; `And_`/`But_` for valid/invalid states; 3-4 levels
- **Key**: Test valid + invalid starting states; see [state-machine](../examples/state-machine.md)

### Event-Driven Workflow
- **When**: System emits events based on analysis
- **Structure**: Root `Given()` initializes event manager; children trigger conditions; 3 levels
- **Key**: Assert event presence/absence and type; reset between scenarios

## Application-Level Patterns

### HTTP Endpoint
- **When**: API behavior end-to-end
- **Structure**: Root `Given()` creates test server + auth + seed; `When()` sends HTTP; 2-3 levels
- **Key**: Test status codes + response body + side effects; see [application-api-endpoint](../examples/application-api-endpoint.md)

## Cross-Cutting Patterns

### Cleanup / Resource Disposal
- **When**: Root `Given()` creates disposable infrastructure (test servers, database connections, HTTP clients)
- **Structure**: Override `Cleanup()` at the bottom of the root spec class; framework calls it via `IDisposable` after tests complete
- **Key**: Placement convention — `Given()` at top (setup), `Cleanup()` at bottom (teardown)

### Typed Exception Capture
- **When**: Assertions need properties from a specific exception subtype (e.g., `ParamName` on `ArgumentException`)
- **Structure**: Use `Catch.Exception<TException>(action)` in `When()` to get the exception already cast; eliminates manual casts in `Should_` methods
- **Key**: Returns `null` if no exception or wrong type — safe for both happy-path and wrong-type scenarios

See also: [exception specification](../examples/exception-specification.md), test data setup, pre-populating SUT state, async Given/When, [test infrastructure context](../examples/application-api-endpoint.md), assertion conventions.
