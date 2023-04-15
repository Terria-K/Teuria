using System;
using System.Collections;

namespace Teuria;


public sealed class StateMachine : Component 
{
    private Func<IEnumerator>?[] coroutineList;
    private Action?[] readyList;
    private Func<int>?[] updateList;
    private Action?[] endList;
    private Coroutine coroutine;

    public int CurrentState 
    {
        get => currentState;
        set 
        {
            SkyLog.Assert(value <= totalStates, $"Total states is {totalStates}, you've reached {value}");
            if (currentState == value) 
                return;
            previousState = currentState;
            currentState = value;
            readyList[currentState]?.Invoke();
            endList[currentState]?.Invoke();
            if (coroutineList[currentState] != null) 
            {
                coroutine.Run(coroutineList[currentState]!());
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
        entity.AddComponent(coroutine);
    }

    internal void AddState(int id, Func<int>? update, Action? ready, Func<IEnumerator>? coroutine, Action? end) 
    {
        updateList[id] = update;
        readyList[id] = ready;
        coroutineList[id] = coroutine;
        endList[id] = end;
    }

    public StateBuilder CreateState(int id) 
    {
        var stateBuilder = new StateBuilder(id, this);
        return stateBuilder;
    }

    public override void Update()
    {
        if (updateList[currentState] != null)
            CurrentState = updateList[currentState]!();
        base.Update();
    }
}

public class StateBuilder 
{
    private int id;
    private Func<IEnumerator>? coroutine;
    private Action? ready;
    private Func<int>? update;
    private Action? end;
    private StateMachine stateMachine;


    internal StateBuilder(int id, StateMachine stateMachine) 
    {
        this.id = id;
        this.stateMachine = stateMachine;
    }

    public StateBuilder AddReady(Action readyFunc) 
    {
        ready = readyFunc;
        return this;
    }

    public StateBuilder AddUpdate(Func<int> updateFunc) 
    {
        update = updateFunc;
        return this;
    }

    public StateBuilder AddEnd(Action endFunc) 
    {
        end = endFunc;
        return this;
    }

    public StateBuilder AddCoroutine(Func<IEnumerator> coroutineFunc) 
    {
        coroutine = coroutineFunc;
        return this;
    }

    public void Build() 
    {
        stateMachine.AddState(id, update, ready, coroutine, end);
        stateMachine = null!;
    }
}