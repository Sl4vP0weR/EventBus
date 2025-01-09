using EventSourcing.Abstractions;
using EventSourcing.Implementations;

namespace EventSourcing.Extensions;

public static class EventSourceExtensions
{
    public static IEventSource<TEvent> AsEventSource<TEvent>(this IEventSource eventSource)
        where TEvent : IEvent => EventSource<TEvent>.For(eventSource);
}