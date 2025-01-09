// ReSharper disable AccessToModifiedClosure
using EventSourcing.Extensions;
using EventSourcing.Implementations;
using EventSourcing.Internal.Implementations;
using EventSourcing.Tests.Events;
using EventSourcing.Tests.Sources;

namespace EventSourcing.Tests.Extensions;

public sealed partial class EventHandlerExtensionsTests : IDisposable
{
    // cleanup
    public void Dispose()
    {
        EventSource<TestEvent>.Global.Clear();
    }
    
    [Fact]
    public static void Listen_HandlerLifetimeDispose_ThrowsInvalidOperation()
    {
        var eventSource = new TestEventSource().AsEventSource<TestEvent>() as EventSource<TestEvent>;
        var @event = new TestEvent();
        Disposable lifetime = null!;

        void Handle(TestEvent @event)
        {
            lifetime!.Dispose(); // deadlock
        }

        var handler = DelegateEventHandler<TestEvent>.From(Handle);

        lifetime = (Disposable)handler.Listen(eventSource);
        
        var act = () => @event.Raise(eventSource);

        act.Should().Throw<InvalidOperationException>("should cancel deadlock");
        
        eventSource.Count.Should().Be(1);
        eventSource.Should().NotBeEmpty();
        eventSource.IsListener(handler).Should().BeTrue();
        
        lifetime.Dispose();
    }
    
    [Fact]
    public static void Listen_HandlerLifetimeDispose_EmptySourceHandlers()
    {
        var eventSource = new TestEventSource().AsEventSource<TestEvent>() as EventSource<TestEvent>;
        var @event = new TestEvent();

        void Handle(TestEvent @event) { }

        var handler = DelegateEventHandler<TestEvent>.From(Handle);

        var lifetime = (Disposable)handler.Listen(eventSource);
        
        var act = () => @event.Raise(eventSource);

        lifetime.Dispose();

        act.Should().NotThrow();

        eventSource.Count.Should().Be(0);
        eventSource.Should().BeEmpty();
        eventSource.IsListener(handler).Should().BeFalse();
    }
    
    [Fact]
    public static void Listen_Null_ReturnsDisposedLifetime()
    {
        var lifetime = (Disposable)EventHandlerExtensions.Listen<TestEvent>(null);
        
        lifetime.IsDisposed.Should().BeTrue();
    }
    
    [Fact]
    public static void Listen_EmptySources_UsesGlobalSource()
    {
        var eventSource = EventSource<TestEvent>.Global;
        
        var @event = new TestEvent();

        void Handle(TestEvent @event) { }

        var handler = DelegateEventHandler<TestEvent>.From(Handle);

        var lifetime = handler.Listen();
        
        var act = () => @event.Raise();

        act.Should().NotThrow();

        eventSource.Count.Should().Be(1);
        eventSource.Should().NotBeEmpty();
        eventSource.IsListener(handler).Should().BeTrue();
        eventSource.IsGlobal.Should().BeTrue();
        
        lifetime.Dispose();
    }
}