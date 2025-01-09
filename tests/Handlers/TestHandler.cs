using EventSourcing.Abstractions;

namespace EventSourcing.Tests.Handlers;

public class TestHandler : IEventHandler<IEvent>
{
    public int Counter { get; private set; }
    
    public void Handle(IEvent @event)
    {
        Counter++;
    }
}