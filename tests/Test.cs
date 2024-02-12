namespace EventBus.Tests;

public abstract class Test
{
    protected readonly ITestOutputHelper Output;
    protected IEventBus Bus = new EventBus();
    
    public Test(ITestOutputHelper output)
    {
        Output = output;
    }
}