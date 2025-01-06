using EventSourcing.Abstractions;
using EventSourcing.Tests.Events;

namespace EventSourcing.Tests.Handlers;

public class TestHandlerA : IEventHandler<TestEventA>
{
    public void Handle(TestEventA @event)
    {
        Console.WriteLine(nameof(TestEventA));
    }
}