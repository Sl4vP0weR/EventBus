namespace EventBus;

/// <summary>
/// Exception that will be handled when event risen and will stop execution of any left listeners.
/// </summary>
public class EventCancellationRequest : Exception
{
    public const string DefaultMessage = "Event cancellation was requested by one of the listeners.";
    
    public EventCancellationRequest(string? message = null) : base(message ?? DefaultMessage) { }
}