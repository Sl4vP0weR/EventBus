namespace EventBus;

/// <summary>
/// Basic implementation of <see cref="IEventListener{TEvent}"/>.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public class EventListener<TEvent> : IEventListener<TEvent>
{
    public EventListener() {}

    public EventListener(Func<TEvent, Task> handler, Action? disposeAction = null) : this()
    {
        Handler = handler;
        DisposeAction = disposeAction;
    }
    
    public EventListener(Action<TEvent> handler) : this(handler.ToTask()) { }
    
    private Func<TEvent, Task>? Handler;
    private Action? DisposeAction;

    public virtual int Order { get; init; }

    public virtual Task Handle(TEvent @event) => Handler?.Invoke(@event) ?? Task.CompletedTask;

    public virtual void Dispose() => DisposeAction?.Invoke();
}