namespace EventBus;

/// <summary>
/// Event bus that raises events and notifies listeners.
/// </summary>
public interface IEventBus : IEventListener
{
    /// <summary>
    /// Listeners collection.
    /// </summary>
    EventListeners Listeners { get; }
    
    /// <summary>
    /// Raises event and notifies listeners to handle it.
    /// </summary>
    /// <param name="event">Event that being risen.</param>
    Task Raise(object @event);

    Task IEventListener.Handle(object @event) => Raise(@event);
}

/// <inheritdoc cref="IEventBus"/>
/// <typeparam name="TEvent">Type of concrete event being handled.</typeparam>
public interface IEventBus<in TEvent> : IEventBus, IEventListener<TEvent>
    where TEvent : IEvent
{
    Task IEventBus.Raise(object @event) => Raise((TEvent)@event);
    
    /// <inheritdoc cref="IEventBus.Raise(Object)"/>
    Task Raise(TEvent @event);

    Task IEventListener.Handle(object @event) => Raise(@event);
    Task IEventListener<TEvent>.Handle(TEvent @event) => Raise(@event);
}