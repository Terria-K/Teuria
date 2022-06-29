using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Teuria;


public class StateMachine<T> : Component 
{
    private Func<Task>[] coroutineList;
    private Action[] readyList;
    private Func<int>[] updateList;
    private Action[] endList;

    private Task coroutineRunning;

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
                coroutineRunning = coroutineList[currentState]();
            }
        }
    }
    private int previousState;
    private int currentState;
    private int totalStates;

    public StateMachine(int amount) 
    {
        previousState = -1;
        totalStates = amount;
        readyList = new Action[amount];
        updateList = new Func<int>[amount];
        coroutineList = new Func<Task>[amount];
        endList = new Action[amount];
    }

    public override void Added(Entity entity)
    {
        CurrentState = 0;
        base.Added(entity);
    }

    public void AddState(int id, Func<int> update = null, Action ready = null, Func<Task> coroutine = null, Action end = null) 
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