using EventSourcing.Abstractions;

namespace EventSourcing.Tests;

public class TestEventSource : IEventSource
{
    public readonly Guid Id = Guid.NewGuid();
}