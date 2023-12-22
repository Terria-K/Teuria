using System;
using System.Threading;
using System.Threading.Tasks;

namespace Teuria;

public class AsyncCoroutine : Component
{
    private CoroutineContext scheduler = new();
    private bool active = true;


    private async Task WrapCoroutine(Func<Task> coroutine)
    {
        await Task.Yield();
        await coroutine();
    }

    public Task Run(Func<Task> coroutine) 
    {
        var oldContext = SynchronizationContext.Current;
        try 
        {
            var syncContext = (SynchronizationContext)scheduler;
            SynchronizationContext.SetSynchronizationContext(syncContext);
            var task = WrapCoroutine(coroutine);
            return task;
        }
        finally 
        {
            SynchronizationContext.SetSynchronizationContext(oldContext);
        }
    }

    public override void Update()
    {
        scheduler.Update();
        base.Update();
    }
}