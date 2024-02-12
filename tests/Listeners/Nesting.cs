namespace EventBus.Tests.Listeners;

public class Nesting : Delegates
{
    public readonly IEventBus Nested = new EventBus();

    public Nesting(ITestOutputHelper output) : base(output) { }

    [Fact]
    public override async Task Raise()
    {
        UserEvent @event;

        var handler = LogListener;

        handler.ToListener(int.MaxValue).Listen(Bus);

        Bus.Listeners.Add(Nested);

        handler = PassListener;
        
        handler.ToListener().Listen(Nested);
        
        using (Nested)
            await (@event = new("123")).Raise(Bus);
        Assert.True(@event.Pass);

        await (@event = new("1234")).Raise(Bus);
        Assert.False(@event.Pass);
    }
}