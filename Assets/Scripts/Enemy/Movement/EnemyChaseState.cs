using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private readonly EnemyMovement2D ctx;

    public EnemyChaseState(EnemyMovement2D context)
    {
        ctx = context;
        StateName = "Chase";
    }

    public override void PhysicsTick()
    {
        if (!ctx.HasPlayerTarget()) return;

        Vector2 toPlayer = ctx.GetToPlayerVector();
        ctx.MoveChasePublic(toPlayer);
        ctx.UpdateLastSeenPlayerPosFromTarget();
    }
}