using EventSourcing.Abstractions;

namespace EventSourcing.Tests.Sources;

public class TestEventSource : IEventSource
{
    public readonly Guid Id = Guid.NewGuid();
}