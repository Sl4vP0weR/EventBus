namespace EventBus;

/// <summary>
/// Event listener proxy with a lifetime management.
/// </summary>
public sealed class ListenerEntry : IEventListener, IComparable<ListenerEntry>
{
    public EventListeners Owner { get; }
    public IEventListener Listener { get; }
    
    public bool Disposed { get; private set; }
    
    public int Order => Listener.Order;

    internal ListenerEntry(EventListeners owner, IEventListener listener)
    {
        Owner = owner;
        Listener = listener;
    }

    /// <exception cref="ObjectDisposedException">Entry was disposed.</exception>
    public Task Handle(object @event)
    {
        if (Disposed) 
            throw new ObjectDisposedException(nameof(ListenerEntry));
        
        return Listener.Handle(@event);
    } 
        
    public void Dispose()
    {
        if(Disposed) return;
        
        Disposed = true;
        
        Owner.RemoveInternal(this);
        Listener.Dispose();
    }

    public int CompareTo(ListenerEntry? other) => Order.CompareTo(other?.Order ?? 0);
}