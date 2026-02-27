public class EnemyPatrolState : EnemyState
{
    private readonly EnemyMovement2D ctx;

    public EnemyPatrolState(EnemyMovement2D context)
    {
        ctx = context;
        StateName = "Patrol";
    }

    public override void Enter()
    {
        base.Enter();
        ctx.OnPatrolSetupPublic();
    }

    public override void PhysicsTick()
    {
        ctx.MovePatrolPublic();
    }
}