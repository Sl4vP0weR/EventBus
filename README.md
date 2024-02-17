## EventBus implementation :bus:

### Event Bus - design pattern for abstract communication between software components.

This design pattern works on the concept of subscriber-publisher interaction, similar to the message brokers. :mailbox:

System does not provide protection from duplicate events invocation.

### Base concepts of implementation:

`IEventListener` - listener/handler of the event. (subscriber) :ear: 

`IEvent` - event object that can be handled by listeners. :black_large_square:

`IEventBus` - bus where any publisher can raise new event, can be used as a listener to replicate passed events. :gear:


### Usage:

1. Declaration of the event. (`IEvent` is a marker interface which allows us to treat object as an event)

```csharp
public class SomeEvent : IEvent
{
    public int SomeParameter;
}
```

2. Bus instantiation.

```csharp
IEventBus bus = new EventBus();
```

3. Listener instantiation and usage.

```csharp
// consider that this code is from the other system component
void LogHandler(SomeEvent @event) =>
    Console.WriteLine("SomeParameter: {0}", @event.SomeParameter);

var handler = LogHandler; // delegate
var listener = handler.ToListener(); // = new EventListener<SomeEvent>(handler);

var listenerEntry = listener.Listen(bus);
```

4. Raising events.

```csharp
SomeEvent @event = new () { SomeParameter = 42 };

await @event.Raise(bus);
```

5. Listener dispose.

```csharp
listenerEntry.Dispose(); // listener entry will not handle any more events from the bus it listens to
```