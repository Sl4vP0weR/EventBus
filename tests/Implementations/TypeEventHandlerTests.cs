using EventSourcing.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Implementations;
using EventSourcing.Tests.Events;
using EventSourcing.Tests.Handlers;
using EventSourcing.Tests.Sources;

namespace EventSourcing.Tests.Implementations;

public class TypeEventHandlerTests
{
    [Fact]
    public void Raise_TestEvent_WithTestHandler_ExecutedExactlyOnce()
    {
        // Arrange
        var eventSource = new TestEventSource();
        var handler = new TestHandler();
        var @event = new TestEvent();
        
        // Act
        handler.Listen(eventSource);
        @event.Raise(eventSource); 

        // Assert
        handler.Counter.Should().Be(1);
    }
    
    [Fact]
    public void Raise_TestEvent_WithTestHandler_NotExecuted()
    {
        // Arrange
        var eventSource = new TestEventSource();
        var handler = new TestHandlerA();
        var @event = new TestEvent();
        
        // Act
        handler.Listen(eventSource);
        @event.Raise(eventSource); 

        // Assert
        handler.Counter.Should().Be(0);
    }
    
    [Fact]
    public void Raise_TestEvent_WithTestHandlerA_ExecutedExactlyOnce()
    {
        // Arrange
        var eventSource = new TestEventSource();
        var handler = new TestHandler();
        var @event = new TestEventA();
        
        // Act
        handler.Listen(eventSource);
        @event.Raise(eventSource); 

        // Assert
        handler.Counter.Should().Be(1);
    }
}