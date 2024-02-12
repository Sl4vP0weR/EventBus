namespace EventBus;

/// <inheritdoc cref="EventBus{TEvent}"/>
public class EventBus : EventBus<IEvent>
{
    /// <summary>
    /// Global event bus that will be used by default.
    /// </summary>
    public static readonly EventBus Global = new();
}

/// <summary>
/// Basic implementation of <see cref="IEventBus{TEvent}"/>.
/// </summary>
/// <typeparam name="TEvent">Type of events that can be risen.</typeparam>
public class EventBus<TEvent> : IEventBus<TEvent>
    where TEvent : IEvent
{   
    public virtual EventListeners Listeners { get; } = [];

    /// <inheritdoc cref="IEventBus.Raise(Object)"/>
    /// <exception cref="AggregateException">Unhandled exceptions from handlers.</exception>
    /// <exception cref="EventCancellationRequest">One of the handlers canceled operation.</exception>
    public virtual async Task Raise(TEvent @event)
    {
        List<Exception> exceptions = new (Listeners.Count);
        
        foreach (var listener in Listeners)
        {
            try
            {
                var task = listener.Handle(@event);
                await task;
            }
            catch (EventCancellationRequest cancellationRequest)
            {
                if (exceptions.Count < 1) throw;
                exceptions.Add(cancellationRequest);
                break;
            }
            catch (Exception exception) { exceptions.Add(exception); }
        }

        if(exceptions.Count > 0)
            throw new AggregateException(exceptions);
    }
}