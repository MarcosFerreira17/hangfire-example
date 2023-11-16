namespace ApplicationOne.Services;
public class HelloWorldService : IHelloWorldService
{
    public Task<string> HelloWorldAsync()
    {
        return Task.FromResult("Hello World!");
    }
}
