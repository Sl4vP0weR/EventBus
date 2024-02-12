using System.Reflection;
using System.Runtime.CompilerServices;

namespace EventBus;

public static class Extensions
{
    public static Func<T, Task> ToTask<T>(this Action<T> handler)
    {
        Task Handler(T obj)
        {
            handler.Invoke(obj);
            return Task.CompletedTask;
        }

        return Handler;
    }
    
    public static Task Raise<TEvent>(this TEvent @event, IEventBus? bus = null)
        where TEvent : IEvent
    {
        bus ??= EventBus.Global;
        return bus.Raise(@event);
    }
    
    public static TaskAwaiter GetAwaiter<TEvent>(this TEvent @event) 
        where TEvent : IEvent => 
        @event.Raise().GetAwaiter();
    
    public static ListenerEntry Listen<TEvent>(this IEventListener<TEvent> listener, IEventBus? bus = null)
        where TEvent : IEvent
    {
        bus ??= EventBus.Global;
        return bus.Listeners.Add(listener);
    }

    public static IEventListener<TEvent> ToListener<TEvent>(this Action<TEvent> handler, int order = 0)
        where TEvent : IEvent =>
        new EventListener<TEvent>(handler) { Order = order };

    public static IEventListener<TEvent> ToListener<TEvent>(this MethodBase method, object? target = null, int order = 0)
        where TEvent : IEvent
    {
        void Handler(TEvent @event) => method.Invoke(target, [@event]);
        var handler = Handler;
        return handler.ToListener(order);
    }

    public static IEventListener<TEvent> ToListener<TEvent>(this Delegate handler, int order = 0)
        where TEvent : IEvent =>
        handler.Method.ToListener<TEvent>(handler.Target, order);
}