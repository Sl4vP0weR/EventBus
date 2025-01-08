using EventSourcing.Abstractions;

namespace EventSourcing.Tests.Handlers;

public class TestHandler : IEventHandler<IEvent>
{
    public void Handle(IEvent @event)
    {
        Console.WriteLine(nameof(IEvent));
    }
}