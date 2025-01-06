// ReSharper disable AccessToModifiedClosure
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using EventSourcing.Abstractions;
using EventSourcing.Exceptions;
using EventSourcing.Extensions;
using EventSourcing.Implementations;
using EventSourcing.Internal.Implementations;
using EventSourcing.Tests.Events;

namespace EventSourcing.Tests.Exceptions;

public sealed class EventCancellationExceptionTests : IDisposable
{
    // cleanup
    public void Dispose()
    {
        EventSource<IEvent>.Global.Clear();
        EventSource<TestEvent>.Global.Clear();
    }

    public static IEnumerable<object[]> EnumerateSources()
    {
        yield return [EventSource<IEvent>.Global];
        yield return [EventSource<TestEvent>.Global];
        yield return [new TestEventSource()];
    }

    [Theory]
    [MemberData(nameof(EnumerateSources))]
    public void RaiseEvent_Exception_StopsProcessingAndThrows(IEventSource source)
    {
        // Arrange
        TestEvent testEvent = new();

        var firstHandlerFlag = Disposable.Empty;
        var secondHandlerFlag = Disposable.Empty;

        async Task FirstHandler(TestEvent ev)
        {
            firstHandlerFlag.Dispose();
            throw new EventCancellationException("Test");
        }

        async Task SecondHandler(TestEvent ev)
        {
            secondHandlerFlag.Dispose(); // should not be executed
        }
        
        EventHandlerExtensions.AsEventHandler<TestEvent>(FirstHandler).Listen(source);
        EventHandlerExtensions.AsEventHandler<TestEvent>(SecondHandler).Listen(source);

        // Act
        var act = () => testEvent.Raise(source);

        // Assert
        act.Should().Throw<EventCancellationException>();
        
        firstHandlerFlag.IsDisposed.Should().BeTrue();
        secondHandlerFlag.IsDisposed.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(EnumerateSources))]
    public void RaiseEvent_NoException_NotThrows(IEventSource source)
    {
        // Arrange
        TestEvent testEvent = new();

        var firstHandlerFlag = Disposable.Empty;
        var secondHandlerFlag = Disposable.Empty;

        async Task FirstHandler(TestEvent ev)
        {
            firstHandlerFlag.Dispose();
        }
        
        async Task SecondHandler(TestEvent ev)
        {
            secondHandlerFlag.Dispose();
        }

        EventHandlerExtensions.AsEventHandler<TestEvent>(FirstHandler).Listen(source);
        EventHandlerExtensions.AsEventHandler<TestEvent>(SecondHandler).Listen(source);
        
        // Act
        testEvent.Raise(source);

        // Assert
        firstHandlerFlag!.IsDisposed.Should().BeTrue();
        secondHandlerFlag!.IsDisposed.Should().BeTrue();
    }
}