namespace EventBus.Tests;

public record UserEvent(string ID) : IEvent
{
    public bool Pass { get; set; }

    public override string ToString() => $"{ID} - {Pass}";
}