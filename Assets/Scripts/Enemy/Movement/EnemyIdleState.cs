using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private readonly EnemyMovement2D ctx;

    public EnemyIdleState(EnemyMovement2D context)
    {
        ctx = context;
        StateName = "Idle";
    }

    public override void Enter()
    {
        base.Enter();
        ctx.StopHorizontalPublic();
    }

    public override void PhysicsTick()
    {
        ctx.StopHorizontalPublic();
    }
}