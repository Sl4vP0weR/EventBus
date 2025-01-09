using EventSourcing.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Implementations;
using EventSourcing.Tests.Events;
using EventSourcing.Tests.Handlers;
using EventSourcing.Tests.Sources;

namespace EventSourcing.Tests.Implementations;

public class EventSourceTests : IDisposable
{
    public void Dispose()
    {
        foreach (var source in EventExtensions.GetGlobalDerivativeEventSources<TestEventA>())
            ((IEventSource<IEvent>)source).Clear();
    }

    [Fact] 
    public void Raise_TestEvent_TestHandlerA_NotExecuted()
    {
        // Arrange
        var handler = new TestHandlerA();
        var @event = new TestEvent();
        
        // Act
        handler.Listen();
        @event.Raise(); 

        // Assert
        handler.Counter.Should().Be(0);
    }
    
    [Fact] 
    public void Raise_TestEventA_TestHandler_GlobalSources_ExecutedCountTimes()
    {
        // Arrange
        var handler = new TestHandler();
        var @event = new TestEventA();
        var derivativeSources = EventExtensions.GetGlobalDerivativeEventSources<TestEventA>().ToHashSet();
        
        // Act
        handler.Listen(derivativeSources);
        @event.Raise(); 

        // Assert
        handler.Counter.Should().Be(derivativeSources.Count);
    }
    
    [Fact] 
    public void Raise_TestEventA_TestHandlerA_GlobalSources_ExecutesMatchedSources()
    {
        // Arrange
        var handler = new TestHandlerA();
        var @event = new TestEventA();
        var derivativeSources = EventExtensions.GetGlobalDerivativeEventSources<TestEventA>().ToHashSet();
        
        // Act
        handler.Listen(derivativeSources);
        @event.Raise(); 

        // Assert
        handler.Counter.Should().Be(1);
    }
    
    [Fact] 
    public void Raise_TestEventA_DerivativeSources_ExecutedCountTimes()
    {
        // Arrange
        var eventSource = new TestEventSource();
        var handler = new TestHandler();
        var @event = new TestEventA();
        var derivativeSources = eventSource.GetDerivativeEventSources<TestEventA>();
        
        // Act
        handler.Listen(derivativeSources.ToHashSet());
        @event.Raise(eventSource); 

        // Assert
        handler.Counter.Should().Be(derivativeSources.Count);
    }
}