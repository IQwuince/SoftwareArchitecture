using System;
using System.Collections.Generic;

public abstract class EnemyState
{
    public string StateName { get; protected set; }
    public Action OnEnter;
    public Action OnExit;

    protected readonly List<EnemyTransition> transitions = new();

    public void AddTransition(Func<bool> condition, EnemyState nextState)
    {
        transitions.Add(new EnemyTransition(condition, nextState));
    }

    public EnemyState GetNextState()
    {
        for (int i = 0; i < transitions.Count; i++)
        {
            if (transitions[i].Condition != null && transitions[i].Condition())
                return transitions[i].NextState;
        }
        return null;
    }

    public virtual void Enter() => OnEnter?.Invoke();
    public virtual void Exit() => OnExit?.Invoke();
    public virtual void Tick() { }       // Update
    public virtual void PhysicsTick() { } // FixedUpdate
}