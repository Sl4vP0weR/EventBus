namespace EventBus.Tests;

public sealed class ReadMeTest : Test
{    
    public ReadMeTest(ITestOutputHelper output) : base(output) { }

    public class SomeEvent : IEvent
    {
        public int SomeParameter;
    }
    
    [Fact]
    public async Task Raise()
    {
        IEventBus bus = new EventBus();
        
        // consider that this code is from the other system component
        void LogHandler(SomeEvent @event) =>
            Output.WriteLine("SomeParameter: {0}", @event.SomeParameter);

        var handler = LogHandler; // delegate
        var listener = handler.ToListener(); // = new EventListener<SomeEvent>(handler);

        var listenerEntry = listener.Listen(bus);
        //
        
        SomeEvent @event = new () { SomeParameter = 42 };

        await @event.Raise(bus);
    }
}