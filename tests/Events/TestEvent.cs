using EventSourcing.Abstractions;

namespace EventSourcing.Tests.Events;

public class TestEvent : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
}