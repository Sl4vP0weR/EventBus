namespace EventBus.Tests.Listeners;

public class Delegates : Test
{    
    public Delegates(ITestOutputHelper output) : base(output) {}

    public void LogListener(UserEvent @event)
    {
        Output.WriteLine(@event.ToString());
    }

    public void PassListener(UserEvent @event)
    {
        @event.Pass = true;
    }

    [Fact]
    public virtual async Task Raise()
    {
        UserEvent @event;

        var handler = LogListener;

        handler.ToListener(int.MaxValue).Listen(Bus);

        handler = PassListener;
        
        var passListener =
            //typeof(Delegates).GetMethod(nameof(PassListener))!.ToListener<UserEvent>(this)
            handler.ToListener()
            .Listen(Bus);
        
        using (passListener)
            await (@event = new("123")).Raise(Bus);
        Assert.True(@event.Pass);

        await (@event = new("1234")).Raise(Bus);
        Assert.False(@event.Pass);
    }
}