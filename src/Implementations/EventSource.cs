using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using EventSourcing.Abstractions;
using EventSourcing.Exceptions;
using EventSourcing.Internal.Extensions;
using Nito.AsyncEx;

namespace EventSourcing.Implementations;

// ReSharper disable once EmptyConstructor
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class EventSource<TEvent>() : IEventSource, IEnumerable<IEventHandler<TEvent>>
    where TEvent : IEvent
{
    public static readonly EventSource<TEvent> Global = new() { IsGlobal = true };
    private static readonly ConditionalWeakTable<IEventSource, EventSource<TEvent>> GlobalPerInstance = [];

    private readonly AsyncReaderWriterLock @lock = new();
    
    private readonly HashSet<IEventHandler<TEvent>> handlers = [];
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay => $"{typeof(TEvent).Name}{(IsGlobal ? " global" : "")} event source, handlers: {Count}";

    public bool IsGlobal { get; private init; }

    public int Count
    {
        get
        {
            using(ReadLock())
                return handlers.Count;
        }
    }
    
    private static CancellationToken TimeoutCancellationToken => 
        new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token;
    
    private static readonly Exception LockTimeoutException = new InvalidOperationException("Possible deadlock was acquired.");

    private IDisposable HandleLockTimeout(Func<CancellationToken, IDisposable> lockFunc)
    {
        try
        {
            var lockHandle = lockFunc(TimeoutCancellationToken);
            return lockHandle;
        }
        catch (OperationCanceledException)
        {
            throw LockTimeoutException;
        }
    }

    private IDisposable ReadLock() => HandleLockTimeout(@lock.ReaderLock);

    private IDisposable WriteLock() => HandleLockTimeout(@lock.WriterLock);
    
    internal void Add(IEventHandler<TEvent> handler)
    {
        if (handler is null)
            return;
        
        using (WriteLock())
        {
            handlers.Add(handler);
        }
    }
    
    internal bool Remove(IEventHandler<TEvent> handler)
    {
        using (WriteLock())
        {
            return handlers.Remove(handler);
        }
    }
    
    internal void Clear()
    {
        using (WriteLock())
        {
            handlers.Clear();
        }
    }

    public bool IsListener(IEventHandler<TEvent> handler)
    {
        using (ReadLock())
        {
            return handlers.Contains(handler);
        }
    }
    
    public IEnumerator<IEventHandler<TEvent>> GetEnumerator()
    {
        using (ReadLock())
        {
            // by design not a sorted collection, ordering on each enumeration is the best way to keep order dynamic
            var enumerator = handlers.OrderBy(x => x.Order).GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current!;
            enumerator.Dispose();
        }
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static EventSource<TEvent> For(IEventSource source)
    {
        if (source is EventSource<TEvent> eventSource)
            return eventSource;

        return GlobalPerInstance.GetOrCreateValue(source)!;
    }

    public async Task HandleAsync(TEvent @event)
    {
        var exceptions = new List<Exception>(Count);

        foreach (var handler in this)
        {
            try
            { 
                if (handler is IAsyncEventHandler<TEvent> asyncHandler)
                    await asyncHandler.HandleAsync(@event);
                else handler.Handle(@event);
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
                
                if (exception is EventCancellationException)
                    break;
            }
        }
        
        exceptions.AggregateExceptions().ThrowIfNotNull();
    }
}