namespace EventBus.Tests.Listeners;

public class Types : Test
{
    public Types(ITestOutputHelper output) : base(output) {}
    
    public class LogListener : IEventListener<UserEvent>
    {
        private ITestOutputHelper output;
        
        public LogListener(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        public int Order => int.MaxValue;
        
        public Task Handle(UserEvent @event)
        {
            output.WriteLine(@event.ToString());
            return Task.CompletedTask;
        }

        public void Dispose() => output.WriteLine(nameof(LogListener)+" Dispose");
    }
    

    public class PassListener : IEventListener<UserEvent>
    {
        private ITestOutputHelper output;
        
        public PassListener(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        public int Order => int.MinValue;

        public Task Handle(UserEvent @event)
        {
            @event.Pass = true;
            return Task.CompletedTask;
        }

        public void Dispose() => output.WriteLine(nameof(PassListener)+" Dispose");
    }

    [Fact]
    public async Task ListenerDisposed()
    {
        UserEvent @event;
        
        var passListener = new PassListener(Output).Listen(Bus);
        
        Assert.False(passListener.Disposed);
        
        using (passListener)
            await (@event = new("123")).Raise(Bus);
        
        Assert.True(passListener.Disposed);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => passListener.Handle(@event));
    }
    
    [Fact]
    public async Task Raise()
    {
        UserEvent @event;

        var logListener = new LogListener(Output).Listen(Bus);

        var passListener = new PassListener(Output).Listen(Bus);
        
        using (passListener)
            await (@event = new("123")).Raise(Bus);
        Assert.True(@event.Pass);
        
        await (@event = new("1234")).Raise(Bus);
        Assert.False(@event.Pass);
    }
}