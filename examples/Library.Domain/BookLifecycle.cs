namespace Library.Domain;

public class BookLifecycle
{
    public BookState CurrentState { get; private set; }

    public BookLifecycle(BookState initialState = BookState.Available)
    {
        CurrentState = initialState;
    }

    public void Lend()
    {
        AssertValidTransition(BookState.Available, BookState.OnLoan);
        CurrentState = BookState.OnLoan;
    }

    public void Return()
    {
        if (CurrentState != BookState.OnLoan && CurrentState != BookState.Overdue)
            throw new InvalidOperationException(
                $"Cannot return a book in state '{CurrentState}'. Must be OnLoan or Overdue.");
        CurrentState = BookState.Available;
    }

    public void MarkOverdue()
    {
        AssertValidTransition(BookState.OnLoan, BookState.Overdue);
        CurrentState = BookState.Overdue;
    }

    public void Reserve()
    {
        AssertValidTransition(BookState.Available, BookState.Reserved);
        CurrentState = BookState.Reserved;
    }

    public void FulfillReservation()
    {
        AssertValidTransition(BookState.Reserved, BookState.OnLoan);
        CurrentState = BookState.OnLoan;
    }

    public void ReportLost()
    {
        if (CurrentState == BookState.Available || CurrentState == BookState.Lost)
            throw new InvalidOperationException(
                $"Cannot report as lost a book in state '{CurrentState}'.");
        CurrentState = BookState.Lost;
    }

    private void AssertValidTransition(BookState requiredState, BookState targetState)
    {
        if (CurrentState != requiredState)
            throw new InvalidOperationException(
                $"Cannot transition to '{targetState}' from '{CurrentState}'. Required state: '{requiredState}'.");
    }
}
