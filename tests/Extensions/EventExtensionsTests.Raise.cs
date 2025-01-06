// ReSharper disable ExpressionIsAlwaysNull

using EventSourcing.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Tests.Events;

namespace EventSourcing.Tests.Extensions;

partial class EventExtensionsTests
{
    [Fact]
    public void Raise_Null_NotThrows()
    {
        // Arrange
        IEvent? @event = null;

        // Act
        var act = () => @event.Raise();

        // Assert
        act.Should().NotThrow();
    }
    
    [Fact]
    public void Raise_TestEvent_NotThrows()
    {
        // Arrange
        IEvent? @event = new TestEvent();

        // Act
        var act = () => @event.Raise();

        // Assert
        act.Should().NotThrow();
    }
}