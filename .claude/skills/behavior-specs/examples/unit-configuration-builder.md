# Configuration Builder with Interacting Parameters

> When to use: a fluent API / Builder where each method configures a parameter that interacts with others — always nest parameters, never flatten

## Key Points
- Production class: `CatalogBuilder` — fluent builder with `WithSection()`, `WithShelfCode()`, `WithName()` configuring a `CatalogEntry` via `Build()`
- Builders/fluent APIs are ALWAYS configuration-style — WRONG: flat `When_` siblings; CORRECT: nest combinatorially
- One terminal action (`Build()`) is the single `When()` at root
- Non-determinant parameter (`catalogName`) set once in root `Given()`, never in context names

## WRONG — Flat (treats builder methods as independent operations)

```
CatalogBuilderSpecs
├── When() → _result = _builder.Build()
├── When_a_custom_section_is_provided          ← flat
├── When_a_custom_shelf_code_is_provided       ← flat
├── When_both_section_and_shelf_are_provided   ← compound name hides matrix
├── When_neither_is_provided                   ← flat
└── When_a_custom_name_is_provided             ← flat
```

## CORRECT — Nested (parameters as nesting dimensions)

```
CatalogBuilderSpecs : FeatureSpecifications
├── Given() → _builder = new CatalogBuilder().WithName("Science")
├── When() → _result = _builder.Build()
├── When_a_custom_section_is_provided
│   ├── Given() → _builder.WithSection("Physics")
│   ├── And_a_shelf_code_is_also_provided       ← present + present
│   │   └── Should_create_a_full_shelf_location()
│   └── But_no_shelf_code_is_provided           ← present + absent
│       └── Should_create_a_section_only_location()
├── When_a_custom_shelf_code_is_provided
│   └── But_no_section_is_provided              ← absent + present
│       └── Should_create_an_unassigned_section_location()
└── When_neither_section_nor_shelf_code_is_provided  ← all-absent standalone

Non-determinant: catalogName (use: "Science")
Leaf count: 4 (section × shelfCode = 2 × 2)
```

## Spec Code

```csharp
[TestFixture]
public class CatalogBuilderSpecs : FeatureSpecifications
{
    protected CatalogBuilder _builder;
    protected CatalogEntry _result;

    public override void Given() =>
        _builder = new CatalogBuilder().WithName("Science");

    public override void When() =>
        _result = _builder.Build();

    public class When_a_custom_section_is_provided : CatalogBuilderSpecs
    {
        public override void Given() => _builder.WithSection("Physics");

        public class And_a_shelf_code_is_also_provided : When_a_custom_section_is_provided
        {
            public override void Given() => _builder.WithShelfCode("A1");

            [Test]
            public void Should_create_a_full_shelf_location() =>
                _result.Location.ShouldBeOfType<ShelfLocation>();
        }

        public class But_no_shelf_code_is_provided : When_a_custom_section_is_provided
        {
            [Test]
            public void Should_create_a_section_only_location() =>
                _result.Location.IsSectionOnly.ShouldBeTrue();
        }
    }

    // When_a_custom_shelf_code_is_provided and When_neither_section_nor_shelf_code_is_provided
    // follow the same nested pattern
}
```
