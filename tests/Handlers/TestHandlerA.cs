using EventSourcing.Abstractions;
using EventSourcing.Tests.Events;

namespace EventSourcing.Tests.Handlers;

public class TestHandlerA : IEventHandler<TestEventA>
{
    public int Counter { get; private set; }
    
    public void Handle(TestEventA @event)
    {
        Counter++;
    }
}