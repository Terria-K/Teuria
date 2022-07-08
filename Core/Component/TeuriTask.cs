using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Teuria;

public class TeuriTask : Component
{
    private Stack<Task> holdingTask = new Stack<Task>();
    private CancellationTokenSource source = new CancellationTokenSource();
    private CancellationToken token;


    public TeuriTask() 
    {
        token = source.Token;
    }
    public void Create(Task task) 
    {
        holdingTask.Push(task);
    }

    public void Create(Func<Task> task) 
    {
        holdingTask.Push(task());
    }


    public void Create(Func<CancellationToken, Task> taskCancellable) 
    {
        holdingTask.Push(taskCancellable(token));
    }

    public void Cancel(bool throwException = false) 
    {
        source.Cancel(throwException);
    }

    public override void Update()
    {
        if (holdingTask.Count <= 0) 
            return;
        var task = holdingTask.Peek();
        if (task.IsCompleted) 
        {
#if DEBUG
            var totalLength = holdingTask.Count;
#endif
            holdingTask.Pop();
            Debug.Assert(totalLength != 0 ? totalLength != holdingTask.Count : true, "It causes Memory Leak");
            task.Dispose();
        }
        base.Update();
    }

    public override void Removed()
    {
        source.Dispose();
        base.Removed();
    }
}