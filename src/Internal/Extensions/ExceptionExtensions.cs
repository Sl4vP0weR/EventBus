using System.Reflection;

namespace EventSourcing.Internal.Extensions;

[ExcludeFromCodeCoverage]
internal static class ExceptionExtensions
{
    public static void ThrowIfNotNull(this Exception? exception)
    {
        if (exception is null)
            return;

        throw exception;
    }

    public static Exception? GetInvocationException(this Exception? exception)
    {
        if (exception is null)
            return null;
        
        if(exception is TargetInvocationException)
            exception = exception.InnerException;
            
        return exception;
    }
    
    public static Exception? AggregateExceptions(this ICollection<Exception> exceptions)
    {
        return exceptions.Count switch
        {
            0 => null,
            1 => throw exceptions.First(),
            _ => throw new AggregateException(exceptions)
        };
    }
}