// ReSharper disable ExpressionIsAlwaysNull

using EventSourcing.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Implementations;
using EventSourcing.Tests.Events;
using EventSourcing.Tests.Sources;

namespace EventSourcing.Tests.Extensions;

partial class EventExtensionsTests
{
    [Fact]
    public void GetDerivativeEventSources_TestEvent_ValidSources()
    {
        // Arrange
        IEventSource eventSource = new TestEventSource();

        IEventSource[] expectedSources =
        [
            EventSource<TestEvent>.For(eventSource),
            EventSource<IEvent>.For(eventSource)
        ];

        // Act
        var derivativeSources = EventExtensions.GetDerivativeEventSources<TestEvent>(eventSource).ToArray();

        // Assert
        derivativeSources.Should().BeEquivalentTo(expectedSources,
            options => options.ComparingByValue<IEventSource>());
    }

    [Fact]
    public void GetDerivativeEventSources_TestEventA_ValidSources()
    {
        // Arrange
        IEventSource eventSource = new TestEventSource();

        IEventSource[] expectedSources =
        [
            eventSource.AsEventSource<TestEventA>(),
            eventSource.AsEventSource<TestEvent>(),
            eventSource.AsEventSource<IEvent>()
        ];

        // Act
        var derivativeSources = eventSource.GetDerivativeEventSources<TestEventA>().ToArray();

        // Assert
        derivativeSources.Should().BeEquivalentTo(expectedSources,
            options => options.ComparingByValue<IEventSource>());
    }

    [Fact]
    public void GetDerivativeEventSources_Null_EmptySources()
    {
        // Arrange
        IEventSource? eventSource = null;

        // Act
        var derivativeSources = EventExtensions.GetDerivativeEventSources<TestEvent>(eventSource).ToHashSet();

        // Assert
        derivativeSources.Should().BeEmpty();
    }
}