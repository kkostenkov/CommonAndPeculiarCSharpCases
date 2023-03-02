using NUnit.Framework;

[TestFixture]
public class Runner_Tests
{
    [Test]
    public void Runner_WaitingAsyncVoidMethods_DontCompile()
    {
        var runner = new Runner();
        // awaiting "async void" methods won't even compile. Try uncommenting
        //await runner.SomeAsyncVoidMethod();
        Assert.IsTrue(true);
    }

    [Test]
    public void Runner_ExceptionIsNotCaught_WhenNotAwaitingAsyncMethods()
    {
        var runner = new Runner();
        Assert.DoesNotThrow(() => runner.SomeThrowingAsyncMethod());
    }

    [Test]
    public void Runner_CatchesException_WhenAsyncMethodIsRunByContinueWith()
    {
        var runner = new Runner();

        runner.SomeThrowingAsyncMethod()
            .ContinueWith((Action<Task>) HandleFaultedTask, TaskContinuationOptions.OnlyOnFaulted);
        
        void HandleFaultedTask(Task faultedTask)
        {
            Assert.IsNotNull(faultedTask.Exception);
        }
    }

    [Test]
    public void Runner_MissesTheException_WhenRunByTaskRun()
    {
        var runner = new Runner();
        Assert.DoesNotThrow(() => Task.Run(runner.SomeThrowingAsyncMethod));
    }
    
    [Test]
    public void Runner_CatchesTheException_WhenRunByTaskRunWithContinueWith()
    {
        var runner = new Runner();
        Assert.Catch(RunTask);

        void RunTask()
        {
            Task.Run(runner.SomeThrowingAsyncMethod)
                // We could wait now, so we any exceptions are thrown, but that 
                // would make the code synchronous. Instead, we continue only if 
                // the task fails.
                .ContinueWith(HandleFaultedTask,
                    default,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext() // do we need a context?
                );
        }
        
        void HandleFaultedTask(Task t)
        {
            // This is always true since we ContinueWith OnlyOnFaulted,
            // But we add the condition anyway so resharper doesn't bark.
            if (t.Exception != null)
                throw t.Exception;
            Assert.IsNotNull(t.Exception);
        }
    }
}