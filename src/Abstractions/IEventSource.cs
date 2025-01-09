namespace EventSourcing.Abstractions;

public interface IEventSource;

public interface IEventSource<out TEvent> : IEventSource
    where TEvent : IEvent
{ 
    void Add(IEventHandler<TEvent> handler);
    bool Remove(IEventHandler<TEvent> handler);
    bool IsListener(IEventHandler<TEvent> handler);
    void Clear();
}