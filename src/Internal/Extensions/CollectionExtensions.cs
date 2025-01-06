namespace EventSourcing.Internal.Extensions;

[ExcludeFromCodeCoverage]
internal static class CollectionExtensions
{
    public static void AddRange<T>(
        this ICollection<T> collection,
        IEnumerable<T> items)
    {
        if(collection.IsReadOnly)
            throw new InvalidOperationException("Collection is read-only.");
        
        if (collection is List<T> list)
        {
            // for some reason, readonly collection is not treated as a collection, but as simple enumerable, to make list grow at one time, this should fix it
            if (items is IReadOnlyCollection<T> readOnlyItems and not ICollection<T>)
                list.Capacity = Math.Max(list.Count + readOnlyItems.Count, list.Capacity);

            list.AddRange(items);
            return;
        }

        foreach (var item in items)
            collection.Add(item);
    }
}