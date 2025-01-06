using System.Reflection;
using System.Runtime.CompilerServices;

namespace EventSourcing.Internal.Extensions;

[ExcludeFromCodeCoverage]
internal static class AsyncExtensions
{
    /// <summary>
    /// Wraps awaitables/awaiters/exceptions/cancellations and other objects into a task.
    /// </summary>
    /// <remarks>
    /// When the <see cref="Task"/> is passed, it will be returned without wrapping.<br/>
    /// This extension exists to wrap a result of any method, no matter synchronous or asynchronous, into a task.
    /// </remarks>
    public static Task AsTask(this object? obj)
    {
        if (obj is null) 
            return Task.CompletedTask;

        if (obj is Task task)
            return task;

        if (obj is Exception exception)
            return Task.FromException(exception);

        if (obj is CancellationToken cancellationToken)
            return Task.FromCanceled(cancellationToken);

        INotifyCompletion? awaiter;

        if (obj is INotifyCompletion notifyCompletion)
            awaiter = notifyCompletion;
        else
        {
            var type = obj.GetType();
            var getAwaiterMethod = type.GetRuntimeMethod(nameof(Task.GetAwaiter), []);
            awaiter = getAwaiterMethod?.Invoke(obj, []) as INotifyCompletion;
        }

        async Task AwaiterTaskWrapper() => await new AwaitableObject(awaiter);
        
        return awaiter is null ? Task.FromResult(obj) : AwaiterTaskWrapper();
    }

    /// <inheritdoc cref="AsTask"/>
    public static Task<TResult> AsTask<TResult>(this object? obj) =>
        obj.AsTask().ContinueWith(task =>
        {
            var type = task.GetType();
            var result = type.GetRuntimeProperty(nameof(Task<TResult>.Result))?.GetValue(task);

            if (result is TResult concreteResult)
                return concreteResult;
            
            return default!;
        });

    private class AwaitableObject
    {
        public readonly AwaiterWrapper awaiterWrapper;
        
        public AwaitableObject(INotifyCompletion awaiter)
        {
            awaiterWrapper = new(awaiter);
        }
        
        public AwaiterWrapper GetAwaiter() => awaiterWrapper;
        
        public class AwaiterWrapper : ICriticalNotifyCompletion
        {
            public readonly INotifyCompletion Awaiter;
            public readonly ICriticalNotifyCompletion? UnsafeAwaiter;
            public readonly Type Type;

            private readonly PropertyInfo? IsCompletedProperty;
            private readonly MethodInfo? GetResultMethod;
            
            public AwaiterWrapper(INotifyCompletion awaiter)
            {
                Awaiter = awaiter;
                Type = awaiter.GetType();
                
                if (awaiter is ICriticalNotifyCompletion unsafeAwaiter)
                    UnsafeAwaiter = unsafeAwaiter;
                
                IsCompletedProperty = Type.GetRuntimeProperty(nameof(IsCompleted));
                GetResultMethod = Type.GetRuntimeMethod(nameof(GetResult), []);
            }
            
            public void UnsafeOnCompleted(Action continuation) => UnsafeAwaiter?.UnsafeOnCompleted(continuation);
                
            public void OnCompleted(Action continuation) => Awaiter.OnCompleted(continuation);

            public bool IsCompleted => IsCompletedProperty?.GetValue(Awaiter) as bool? ?? true;
 
            public void GetResult() => GetResultMethod?.Invoke(Awaiter, []);
        }
    }
}