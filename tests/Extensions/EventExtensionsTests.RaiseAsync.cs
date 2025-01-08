// ReSharper disable ExpressionIsAlwaysNull

using EventSourcing.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Tests.Events;

namespace EventSourcing.Tests.Extensions;

partial class EventExtensionsTests
{
    [Fact]
    public async Task RaiseAsync_Null_NotThrows()
    {
        // Arrange
        IEvent? @event = null;

        // Act
        var act = () => @event.RaiseAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task RaiseAsync_TestEvent_NotThrows()
    {
        // Arrange
        IEvent? @event = new TestEvent();

        // Act
        var act = () => @event.RaiseAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task RaiseAsync_NullSource_NotThrows()
    {
        // Arrange
        IEvent? @event = new TestEvent();

        // Act
        var act = () => @event.RaiseAsync([null!]);

        // Assert
        await act.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task RaiseAsync_NullSources_NotThrows()
    {
        // Arrange
        IEvent? @event = new TestEvent();

        // Act
        var act = () => @event.RaiseAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}