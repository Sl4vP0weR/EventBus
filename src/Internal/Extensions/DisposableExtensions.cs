using EventSourcing.Implementations;
using EventSourcing.Internal.Implementations;

namespace EventSourcing.Internal.Extensions;

[ExcludeFromCodeCoverage]
internal static class DisposableExtensions
{
    public static Disposable Enrich(this IDisposable? disposable) =>
        new(disposable is not null ? disposable.Dispose : null);

    public static Disposable ToDisposable(this Action? action) => new(action);

    public static IDisposable Chain(this IDisposable? @this, params IDisposable?[] disposables)
    {
        if (disposables.Length < 1)
            return @this ?? Disposable.Disposed;

        void Dispose()
        {
            @this?.Dispose();

            foreach (var disposable in disposables)
                disposable?.Dispose();
        }

        return new Disposable(Dispose);
    }
}