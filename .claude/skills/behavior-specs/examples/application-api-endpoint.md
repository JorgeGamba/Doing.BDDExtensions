# Application-Level API Endpoint Specification

> When to use: specifying HTTP endpoints end-to-end — request in `When()`, response assertions in `[Test]`, infrastructure in root `Given()`

## Key Points
- Production class: Library HTTP API — endpoints for borrowing and listing books with authentication/authorization
- Root `Given()` starts test server and seeds data; child `Given()` overrides only what changes
- `When()` issues the HTTP request — one request per `When_` class
- Use `[TearDown]` on contexts that mutate shared state (e.g., auth) to restore after run
- Hierarchy intentionally shallow (2-3 levels) for application specs
- `And_` for request variations with same outcome; `But_` for error/rejection contexts
- **Fractal chain**: `Should_` here (e.g., loan produced) traces to `When_` at module level (LendingServiceSpecs)

## Spec Code

```csharp
// Illustrative example — shows the GWT application-level pattern for an HTTP API.

[TestFixture]
public class LibraryAPISpecs : FeatureSpecifications
{
    protected TestLibraryServer _server;
    protected HttpResponseMessage _response;
    protected Member _authenticatedMember;

    public override void Given()
    {
        _server = new TestLibraryServer();
        _authenticatedMember = _server.SeedMember("Alice", MembershipTier.Standard);
        _server.SeedBook("B001", "Clean Code", BookState.Available);
        _server.AuthenticateAs(_authenticatedMember);
    }

    public class When_borrowing_a_book : LibraryAPISpecs
    {
        public override void When() => _response = _server.Post("/books/B001/borrow");

        public class And_the_book_is_available : When_borrowing_a_book
        {
            [Test]
            public void Should_return_200_OK() => ((int)_response.StatusCode).ShouldBe(200);

            [Test]
            public void Should_include_the_loan_record_in_the_body()
            {
                var body = _response.ReadJson();
                body["bookId"].ShouldBe("B001");
                body["status"].ShouldBe("OnLoan");
            }
        }

        public class But_the_book_is_already_on_loan : When_borrowing_a_book
        {
            public override void Given() => _server.SetBookState("B001", BookState.OnLoan);

            [Test]
            public void Should_return_409_Conflict() => ((int)_response.StatusCode).ShouldBe(409);
        }

        // But_the_member_is_not_authenticated (401 + [TearDown] for auth restore),
        // But_the_book_does_not_exist (404), When_listing_books follow the same pattern
    }
}
```
