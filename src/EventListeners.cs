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
    public ListenerEntry Add(IEventListener listener, bool distinct = false)
    {
        ListenerEntry proxy;

        if (distinct)
        {
            proxy = listeners.FirstOrDefault(x => x.Listener == listener);
            if (proxy is not null)
                return proxy;
        }
        
        proxy = new(this, listener);

        listeners.Add(proxy);
        return proxy;
    }

    /// <summary>
    /// Removes all listeners from the collection.
    /// </summary>
    public void Clear()
    {
        while (Count > 0)
        {
            var listener = listeners.First();
            listener.Dispose();
        }
    }

    internal bool RemoveInternal(ListenerEntry listener) => listeners.Remove(listener);
}