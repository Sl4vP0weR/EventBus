namespace EventSourcing.Abstractions;

public interface IEventHandler
{
    public int Order => 0;
}

public interface IEventHandler<TEvent> : IEventHandler
    where TEvent : IEvent
{
    void Handle(TEvent @event);
}