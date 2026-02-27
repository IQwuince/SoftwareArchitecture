using UnityEngine;

public class EnemySearchState : EnemyState
{
    private readonly EnemyMovement2D ctx;

    public EnemySearchState(EnemyMovement2D context)
    {
        ctx = context;
        StateName = "Search";
    }

    public override void Enter()
    {
        base.Enter();
        ctx.PrepareSearchSnapshot();
    }

    public override void PhysicsTick()
    {
        if (ctx.SearchPointsCount == 0)
        {
            if (ctx.IsSearchTimedOut())
                return; // transition handles leaving state
            ctx.StopHorizontalPublic();
            return;
        }

        if (ctx.SearchIndex >= ctx.SearchPointsCount)
            return; // transition handles leaving state

        Vector2 target = ctx.GetSearchPointAt(ctx.SearchIndex);

        if (ctx.HasReachedTargetPublic(target))
        {
            ctx.AdvanceSearchIndex();
            ctx.StopHorizontalPublic();
            return;
        }

        ctx.MoveSearchTowardPublic(target);
    }
}