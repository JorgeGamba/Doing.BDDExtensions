# Exception Specification Patterns

> When to use: subject method is expected to throw under certain inputs — or you need to prove it does NOT throw

## Key Points
- Production class: `OrderService` — handles order placement and cancellation with validation rules
- Always capture via `Catch.Exception(...)` in `When()` — never `Assert.Throws` or try/catch
- Store in `protected Exception _exception` on nearest common ancestor
- `_exception.ShouldBeNull()` is the explicit no-throw assertion for happy-path contexts
- Cast to concrete type only for property assertions beyond `Message` (e.g., `ParamName`)
- Sibling `When_` classes can each produce different exception types — `_exception` is reused

## Spec Code

```csharp
[TestFixture]
public class OrderServiceSpecs : FeatureSpecifications
{
    protected OrderService _service;
    protected Exception _exception;

    public override void Given() => _service = new OrderService(new InMemoryInventory());

    // Pattern 1: Assert exception type
    public class When_placing_an_order_with_a_null_product_id : OrderServiceSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(() => _service.PlaceOrder(productId: null, quantity: 1));

        [Test]
        public void Should_throw_ArgumentNullException() =>
            _exception.ShouldBeOfType<ArgumentNullException>();
    }

    // Pattern 2: Typed capture (no manual cast needed)
    public class When_placing_an_order_with_a_negative_quantity : OrderServiceSpecs
    {
        public override void When() =>
            _typedException = Catch.Exception<ArgumentOutOfRangeException>(
                () => _service.PlaceOrder("SKU-01", quantity: -1));

        [Test]
        public void Should_name_the_quantity_parameter() =>
            _typedException.ParamName.ShouldBe("quantity");

        ArgumentOutOfRangeException _typedException;
    }

    // Pattern 3: Assert message content
    public class When_placing_an_order_that_exceeds_stock : OrderServiceSpecs
    {
        public override void Given()
        {
            base.Given();
            _service.Inventory.SetStock("SKU-01", 3);
        }

        public override void When() =>
            _exception = Catch.Exception(() => _service.PlaceOrder("SKU-01", quantity: 10));

        [Test]
        public void Should_mention_the_product() => _exception.Message.ShouldContain("SKU-01");
    }

    // Pattern 4: Assert no exception (happy path)
    public class When_placing_a_valid_order : OrderServiceSpecs
    {
        public override void Given()
        {
            base.Given();
            _service.Inventory.SetStock("SKU-01", 100);
        }

        public override void When() =>
            _exception = Catch.Exception(() => _service.PlaceOrder("SKU-01", quantity: 5));

        [Test]
        public void Should_not_throw() => _exception.ShouldBeNull();
    }

    // Pattern 5: Multiple sibling contexts with different exception types —
    // When_cancelling_an_order_that_does_not_exist (KeyNotFoundException) etc.

    // Pattern 6: Async exception capture (Func<Task> overload)
    public class When_processing_a_null_order_asynchronously : OrderServiceSpecs
    {
        public override void When() =>
            _exception = Catch.Exception(async () => await _service.ProcessAsync(null));

        [Test]
        public void Should_throw_ArgumentNullException() =>
            _exception.ShouldBeOfType<ArgumentNullException>();
    }
}
```
