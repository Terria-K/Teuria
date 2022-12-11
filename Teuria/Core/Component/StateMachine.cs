using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Teuria;


public class StateMachine : Component 
{
    private Func<IEnumerator>[] coroutineList;
    private Action[] readyList;
    private Func<int>[] updateList;
    private Action[] endList;
    private Coroutine coroutine;
    // private TeuriTask task;
    // private Task coroutineRunning;

    public int CurrentState 
    {
        get => currentState;
        set 
        {
            Debug.Assert(value <= totalStates, $"Total states is {totalStates}, you've reached {value}");
            if (currentState == value) 
                return;
            previousState = currentState;
            currentState = value;
            readyList[currentState]?.Invoke();
            endList[currentState]?.Invoke();
            if (coroutineList[currentState] != null) 
            {
                coroutine.Run(coroutineList[currentState]());
            }
        }
    }
    private int previousState;
    private int currentState;
    private int totalStates;

    public StateMachine(int amount) 
    {
        coroutine = new Coroutine();
        previousState = -1;
        totalStates = amount;
        readyList = new Action[amount];
        updateList = new Func<int>[amount];
        coroutineList = new Func<IEnumerator>[amount];
        endList = new Action[amount];
    }

    public override void Added(Entity entity)
    {
        CurrentState = 0;
        base.Added(entity);
        Add(coroutine);
    }

    public void AddState(int id, Func<int> update = null, Action ready = null, Func<IEnumerator> coroutine = null, Action end = null) 
    {
        updateList[id] = update;
        readyList[id] = ready;
        coroutineList[id] = coroutine;
        endList[id] = end;
    }

    public override void Update()
    {
        if (updateList[currentState] != null)
            CurrentState = updateList[currentState]();
        base.Update();
    }
}