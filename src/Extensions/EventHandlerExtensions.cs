using EventSourcing.Abstractions;
using EventSourcing.Implementations;
using EventSourcing.Internal.Extensions;
using EventSourcing.Internal.Implementations;

namespace EventSourcing.Extensions;

public static class EventHandlerExtensions
{
    public static DelegateEventHandler<TEvent> AsEventHandler<TEvent>(this Delegate @delegate)
        where TEvent : IEvent =>
        DelegateEventHandler<TEvent>.From(@delegate);

    public static IDisposable Listen<TEvent>(this IEventHandler<TEvent>? handler,
        params HashSet<IEventSource> sources)
        where TEvent : IEvent => Listen(handler, sources.Select(x => x.AsEventSource<TEvent>()).ToHashSet());

    private static Disposable Listen<TEvent>(IEventHandler<TEvent>? handler,
        params HashSet<IEventSource<TEvent>> sources)
        where TEvent : IEvent
    {
        if (handler is null)
            return Disposable.Disposed;

        sources.Remove(null!);

        if (sources.Count < 1)
            sources.Add(EventSource<TEvent>.Global);

        foreach (var source in sources)
            source.Add(handler);

        void Dispose()
        {
            foreach (var source in sources)
                source.Remove(handler);
        }

        var dispose = Dispose;

        return dispose.ToDisposable();
    }
}