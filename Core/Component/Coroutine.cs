using System.Collections.Generic;
using System.Collections;

namespace Teuria;

public class Coroutine : Component 
{
    private Stack<IEnumerator> coroutines = new Stack<IEnumerator>();
    private float timer;
    private bool done;

    public Coroutine() {}

    public Coroutine(IEnumerator corou) 
    {
        coroutines.Push(corou);
    }

    public RefCoroutine Run(IEnumerator coroutine) 
    {
        Active = true;
        timer = 0f;
        coroutines.Clear();
        coroutines.Push(coroutine);
        return new RefCoroutine(this, coroutine);
    }

    public bool IsYielding(IEnumerator coroutine) 
    {
        return coroutines.Contains(coroutine);
    }

    public bool IsYielding(in RefCoroutine coroutine) 
    {
        return coroutine.IsRunning;
    }

    public void Cancel() 
    {
        Active = false;
        timer = 0;
        coroutines.Clear();
        done = true;
    }

    public override void Update()
    {
        UpdateCoroutine();
        base.Update();
    }

    private void UpdateCoroutine() 
    {
        done = false;

        if (timer > 0) 
        {
            timer -= TeuriaEngine.DeltaTime;
            return;
        }
        if (coroutines.Count == 0) return;
        IEnumerator current = coroutines.Peek();
        if (current != null && current.MoveNext() && !done) 
        {
            CheckReturnType(ref current);
            return;
        }
        if (!done)
            Final();   
    }

    private void Final() 
    {
        coroutines.Pop();
        if (coroutines.Count == 0) 
        {
            Active = false;
        }
    }

    private void CheckReturnType(ref IEnumerator current) 
    {
        if (current.Current is float single) 
        {
            timer = single;
            return;
        }
        else if (current.Current is int integer) 
        {
            timer = integer;
            return;
        }
        else if (current.Current is IEnumerator corou) 
        {
            coroutines.Push(corou);
        }
    }
}