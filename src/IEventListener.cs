namespace EventBus;

/// <summary>
/// Listener of any event.
/// </summary>
public interface IEventListener : IDisposable
{
    /// <summary>
    /// Execution order of current listener.
    /// </summary>
    int Order => 0;

    /// <summary>
    /// Handles the event when it's being raised on the listening bus.
    /// </summary>
    Task Handle(object @event);
    
    /// <summary>
    /// Handles listener disposal.
    /// </summary>
    void IDisposable.Dispose() {}
}

/// <summary>
/// Listener of <typeparamref name="TEvent" />.
/// </summary>
/// <typeparam name="TEvent">Type of concrete event being handled.</typeparam>
public interface IEventListener<in TEvent> : IEventListener
{
    Task IEventListener.Handle(object @event) => Handle((TEvent)@event);
    
    /// <inheritdoc cref="IEventListener.Handle(Object)"/>
    Task Handle(TEvent @event);
}