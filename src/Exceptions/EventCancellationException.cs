namespace EventSourcing.Exceptions;

/// <summary>
/// Exception that will be handled when event risen and will stop execution of any left handlers.
/// </summary>
public class EventCancellationException(string? message = null) : Exception(message ?? DefaultMessage)
{
    public const string DefaultMessage = "Event cancellation was requested by one of the handlers.";
}