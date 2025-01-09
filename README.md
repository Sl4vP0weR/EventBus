# EventSourcing

EventSourcing is a .NET library that implements basis for event sourcing pattern.

Documentation is currently W.I.P.

## Installation

Install the package via .NET CLI:

```bash
dotnet add package Sl4vP0weR.EventSourcing
```
## Usage

### Defining Event Sources
Define type that can be a source for events.
```csharp
public record User(string Name, string Surname, string PhoneNumber) : IEventSource
```

### Defining Events
Define event representing state changes:
```csharp
public record UserCreatedEvent(DateTime CreatedAt, User User) : IEvent;
```

### Defining Event Handlers
Define event handler as a type to listen events:
```csharp
// suitable for any type of event.
public class AuditEventHandler : IEventHandler<IEvent>
{
    public int Order => int.MaxValue; // should run after other handlers
    
    public void Handle(IEvent @event) => PerformAudit(@event);
    
    private void PerformAudit(IEvent @event) 
    {
         // save to the file/database, or send as a message to the message broker/web api, or whatever suits your needs.
        ...
    }
}
```

### Handling events
Create a method event handler instance:
```csharp
IEventHandler<IEvent> handler = DelegateEventHandler<IEvent>.From(PerformAudit);
```
or class event handler instance:
```csharp
IEventHandler<IEvent> handler = new AuditEventHandler();
```
and listen to the incoming global events:
```csharp
handler.Listen();
```
or to the specific source
```csharp
IEventSource user = new User("Frederic", "Chopin", "123456789");

handler.Listen(user);
```

### Raising events
Create an event:
```csharp
User user = new User("Frederic", "Chopin", "123456789");

var @event = new UserCreatedEvent(DateTime.UtcNow, user);
```

Raise event:
```csharp
@event.Raise();
```
or with specific sources (still raising global):
```csharp
@event.Raise(user);
```
or asynchronous approach:
```csharp
await @event.RaiseAsync(user);
```

## License
This project is licensed under the MIT License.<br/> See the LICENSE file for details.