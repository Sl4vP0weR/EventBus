using EventSourcing.Abstractions;
using EventSourcing.Internal.Extensions;
using JetBrains.Annotations;

namespace EventSourcing.Implementations;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class DelegateEventHandler<TEvent> : IAsyncEventHandler<TEvent>
    where TEvent : IEvent
{   
    [ExcludeFromCodeCoverage]
    public int Order { get; set; }
    
    private readonly Delegate @delegate;

    private DelegateEventHandler(Delegate @delegate)
    {
        this.@delegate = @delegate;
    }
    
    public static DelegateEventHandler<TEvent> From(Delegate @delegate)
    {
        if(@delegate is null)
            throw new ArgumentNullException(nameof(@delegate));
        
        var method = @delegate.Method;
        var parameters = method.GetParameters();
        
        if(parameters is not [ { } parameter ] || 
           parameter.IsOut ||
           !parameter.ParameterType.IsAssignableFrom(typeof(TEvent)))
            throw new ArgumentException($"Delegate must have single input parameter assignable to type '{typeof(TEvent).Name}'.");
        
        return new(@delegate);
    }
    
    [ExcludeFromCodeCoverage]
    public static implicit operator DelegateEventHandler<TEvent>(Delegate @delegate) => From(@delegate);

    public Task HandleAsync(TEvent @event)
    {
        object? result;
        try
        {
            result = @delegate.Method.Invoke(@delegate.Target, [@event]);
        }
        catch (Exception exception)
        {
            return Task.FromException(exception.GetInvocationException()!);
        }

        return result.AsTask();
    }

    public override int GetHashCode() => HashCode.Combine(@delegate.Target, @delegate.Method);
}