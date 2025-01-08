using EventSourcing.Abstractions;
using EventSourcing.Implementations;

namespace EventSourcing.Extensions;

public static class EventSourceExtensions
{
    public static EventSource<TEvent> AsEventSource<TEvent>(this IEventSource eventSource)
        where TEvent : IEvent => EventSource<TEvent>.For(eventSource);
}