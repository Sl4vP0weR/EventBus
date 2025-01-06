using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using EventSourcing.Abstractions;
using EventSourcing.Exceptions;
using EventSourcing.Implementations;
using EventSourcing.Internal.Extensions;
using Nito.AsyncEx;

namespace EventSourcing.Extensions;

public static class EventExtensions
{
    public static void Raise<TEvent>(this TEvent? @event, params HashSet<IEventSource> sources)
        where TEvent : IEvent =>
        AsyncContext.Run(() => RaiseAsync(@event, sources));

    public static async Task RaiseAsync<TEvent>(this TEvent? @event, params HashSet<IEventSource> sources)
        where TEvent : IEvent
    {
        if (@event is null) return;
        
        if(sources is null)
            throw new ArgumentNullException(nameof(sources));
        
        sources.Remove(null!);
        
        sources.AddRange(sources.SelectMany(GetDerivativeEventSources<TEvent>).ToArray());

        sources.AddRange(GetGlobalDerivativeEventSources<TEvent>());

        var exceptions = new List<Exception>(sources.Count);

        foreach (var source in sources)
        {
            var sourceType = source.GetType();

            if (!sourceType.IsGenericType)
                continue;

            var genericDefinition = sourceType.GetGenericTypeDefinition();

            if (genericDefinition != typeof(EventSource<>))
                continue;

            var eventType = sourceType.GetGenericArguments().First();
            var @delegate = EventSource<TEvent>.Global.HandleAsync;
            var method = @delegate.Method.SwapTypeGenericArguments(eventType);

            if (method is null)
                continue;

            try
            {
                var task = method.Invoke(source, [@event]) as Task;

                await (task ?? Task.CompletedTask);
            }
            catch (Exception exception)
            {
                exception = exception.GetInvocationException()!;
                
                exceptions.Add(exception);
                
                if(exception is EventCancellationException)
                    break;
            }
        }

        exceptions.AggregateExceptions().ThrowIfNotNull();
    }

    private static readonly ConditionalWeakTable<IEventSource, IReadOnlyCollection<IEventSource>> derivativeEventSourcesCache = [];
    public static IReadOnlyCollection<IEventSource> GetDerivativeEventSources<TEvent>(this IEventSource? eventSource)
        where TEvent : IEvent
    {
        if (eventSource is null) return [];

        return derivativeEventSourcesCache.GetValue(eventSource, x => GetDerivativeEventSourcesInternal<TEvent>(x).ToImmutableArray())!;
    }
    
    private static IEnumerable<IEventSource> GetDerivativeEventSourcesInternal<TEvent>(IEventSource? eventSource)
        where TEvent : IEvent
    {
        if (eventSource is null) yield break;
        
        var types = typeof(TEvent).GetAllDerivativeTypes(typeof(IEvent));

        foreach (var eventType in types)
        {
            var @delegate = EventSource<TEvent>.For;
            var method = @delegate.Method.SwapTypeGenericArguments(eventType);
            var result = method?.Invoke(null, [eventSource]);

            if (result is IEventSource source)
                yield return source;
        }
    }

    private static readonly Dictionary<Type, IReadOnlyCollection<IEventSource>> globalEventSourcesCache = [];
    public static IReadOnlyCollection<IEventSource> GetGlobalDerivativeEventSources<TEvent>()
        where TEvent : IEvent
    {
        lock (globalEventSourcesCache)
        {
            if (globalEventSourcesCache.TryGetValue(typeof(TEvent), out var sources))
                return sources;
            
            sources = GetGlobalDerivativeEventSourcesInternal<TEvent>().ToArray();

            globalEventSourcesCache[typeof(TEvent)] = sources;
            
            return sources;
        }
    }
    
    private static IEnumerable<IEventSource> GetGlobalDerivativeEventSourcesInternal<TEvent>()
        where TEvent : IEvent
    {
        var types = typeof(TEvent).GetAllDerivativeTypes(typeof(IEvent));
        foreach (var eventType in types)
        {
            var eventSourceType = typeof(EventSource<>).MakeGenericType(eventType);
            var field = eventSourceType.GetRuntimeField(nameof(EventSource<TEvent>.Global));
            var value = field?.GetValue(null);

            if (value is IEventSource source)
                yield return source;
        }
    }
}