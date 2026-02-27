using System;

public class EnemyTransition
{
    public Func<bool> Condition { get; }
    public EnemyState NextState { get; }

    public EnemyTransition(Func<bool> condition, EnemyState nextState)
    {
        Condition = condition;
        NextState = nextState;
    }
}