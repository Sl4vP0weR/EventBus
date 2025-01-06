using EventSourcing.Implementations;
using EventSourcing.Tests.Events;
// ReSharper disable AssignNullToNotNullAttribute

namespace EventSourcing.Tests.Implementations;

public class DelegateEventHandlerTests
{
    [Fact]
    public void From_ObjectParameterDelegate_NotThrows()
    {
        // Arrange
        void Handler(object @event) { }
        
        // Act
        var act = () => DelegateEventHandler<TestEvent>.From(Handler);

        // Assert
        act.Should().NotThrow();
    }
    
    [Fact]
    public void From_ValidDelegate_NotThrows()
    {
        // Arrange
        void Handler(TestEvent @event) { }
        
        // Act
        var act = () => DelegateEventHandler<TestEvent>.From(Handler);

        // Assert
        act.Should().NotThrow();
    }
    
    [Fact]
    public void From_InvalidDelegate_ThrowsInvalidArgument()
    {
        // Arrange
        void Handler(TestEventA @event) { }
        
        // Act
        var act = () => DelegateEventHandler<TestEvent>.From(Handler);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void From_Null_ThrowsArgumentNull()
    {
        // Arrange
        Delegate? @delegate = null;
        
        // Act
        var act = () => DelegateEventHandler<TestEvent>.From(@delegate);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}