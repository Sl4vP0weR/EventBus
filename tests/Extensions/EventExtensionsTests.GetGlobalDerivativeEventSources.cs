using EventSourcing.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Implementations;
using EventSourcing.Tests.Events;

namespace EventSourcing.Tests.Extensions;

partial class EventExtensionsTests
{
    [Fact]
    public void GetGlobalDerivativeEventSources_TestEvent_ValidSources()
    {
        // Arrange
        IEventSource[] expectedSources =
        [
            EventSource<TestEvent>.Global,
            EventSource<IEvent>.Global
        ];

        // Act
        var derivativeSources = EventExtensions.GetGlobalDerivativeEventSources<TestEvent>().ToArray();

        // Assert
        derivativeSources.Should().BeEquivalentTo(expectedSources,
            options => options.ComparingByValue<IEventSource>());
    }

    [Fact]
    public void GetGlobalDerivativeEventSources_TestEventA_ValidSources()
    {
        // Arrange
        IEventSource[] expectedSources =
        [
            EventSource<TestEventA>.Global,
            EventSource<TestEvent>.Global,
            EventSource<IEvent>.Global
        ];

        // Act
        var derivativeSources = EventExtensions.GetGlobalDerivativeEventSources<TestEventA>().ToArray();

        // Assert
        derivativeSources.Should().BeEquivalentTo(expectedSources,
            options => options.ComparingByValue<IEventSource>());
    }
    
    [Fact]
    public void GetGlobalDerivativeEventSources_GenericEvent_ValidSources()
    {
        // Arrange
        IEventSource[] expectedSources =
        [
            EventSource<IEvent>.Global
        ];

        // Act
        var derivativeSources = EventExtensions.GetGlobalDerivativeEventSources<IEvent>().ToArray();

        // Assert
        derivativeSources.Should().BeEquivalentTo(expectedSources,
            options => options.ComparingByValue<IEventSource>());
    }
}