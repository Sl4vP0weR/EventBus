namespace EventBus;

/// <summary>
/// Collection of the listeners entries, owns each entry.
/// </summary>
public sealed class EventListeners : IEnumerable<ListenerEntry>
{
    private readonly SortedSet<ListenerEntry> listeners = [];
    
    /// <summary>
    /// Listeners count.
    /// </summary>
    public int Count => listeners.Count;
    
    public IEnumerator<ListenerEntry> GetEnumerator() => listeners.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Adds new listener to the collection.
    /// </summary>
    public ListenerEntry Add(IEventListener listener)
    {
        // if entry exists, return it
        // if listener entry already passed, we return it
        listener = listeners.FirstOrDefault(x => x.Listener == listener) ?? listener; 
        
        if (listener is not ListenerEntry proxy)
        {
            proxy = new(this, listener);

            listeners.Add(proxy);
        }

        return proxy;
    }

    /// <summary>
    /// Removes all listeners from the collection.
    /// </summary>
    public void Clear()
    {
        foreach(var listener in listeners)
            listener.Dispose();
        listeners.Clear();
    }

    internal bool RemoveInternal(ListenerEntry listener) => listeners.Remove(listener);
}