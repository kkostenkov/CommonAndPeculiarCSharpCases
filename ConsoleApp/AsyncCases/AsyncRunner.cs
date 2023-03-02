public class Runner
{
    public async void SomeThrowingAsyncVoidMethod()
    {
        await Task.Run(ThrowException);
    }
    
    public async Task SomeThrowingAsyncMethod()
    {
        await Task.Run(ThrowException);
    }
    
    public async Task SomeAsyncMultitaskMethod()
    {
        var waitFor = Task.Delay(TimeSpan.FromSeconds(1));
        var throwTask = Task.Run(ThrowException);
        var all = Task.WhenAll(waitFor, throwTask);
        await all;
    }

    public void ThrowException()
    {
        Console.WriteLine($"Crash on thread #{System.Environment.CurrentManagedThreadId}");
        throw new Exception("Will you see me crashing?");
        Console.WriteLine("AfterCrash");
    }
}