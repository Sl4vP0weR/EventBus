using Nito.AsyncEx;

namespace EventSourcing.Abstractions;

public interface IAsyncEventHandler<TEvent> : IEventHandler<TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent @event);

    void IEventHandler<TEvent>.Handle(TEvent @event) => AsyncContext.Run(() => HandleAsync(@event));
}