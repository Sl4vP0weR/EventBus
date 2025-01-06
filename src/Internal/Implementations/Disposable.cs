using System.Diagnostics;
using EventSourcing.Internal.Extensions;

namespace EventSourcing.Internal.Implementations;

[ExcludeFromCodeCoverage]
[DebuggerDisplay("{DebuggerDisplay}")]
internal sealed class Disposable : IDisposable
{
    private Action? action;

    public Disposable(CancellationToken cancellationToken)
    {
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        CancellationToken = cancellationTokenSource.Token;
    }

    public Disposable(Action? disposeAction = null) : this(CancellationToken.None)
    {
        action = disposeAction;
    }

    public bool IsDisposed => CancellationToken.IsCancellationRequested;

    public static Disposable Empty => new();

    public static readonly Disposable Disposed = new(new CancellationToken(true));

    private readonly CancellationTokenSource cancellationTokenSource;

    public CancellationToken CancellationToken { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => IsDisposed ? "Disposed" : "";
    
    public void Dispose()
    {
        if (IsDisposed) return;

        cancellationTokenSource.Cancel();
        action?.Invoke();
    }

    public static implicit operator Disposable(Action? action) => action.ToDisposable();
    
    public static implicit operator bool(Disposable disposable) => disposable.IsDisposed;
    
    public static implicit operator Disposable(bool disposed) => disposed ? Disposed : Empty;
}