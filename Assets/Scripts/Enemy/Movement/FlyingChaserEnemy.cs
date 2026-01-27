using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingChaserEnemy : EnemyMovement2D
{
    [Header("Flying Patrol Settings")]
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float verticalSpeedMultiplier = 1f;
    [SerializeField] private float wallCheckDistance = 0.35f;
    [SerializeField] private float patrolY = 0f; // if 0, uses start Y

    private Vector2 patrolCenter;
    private int patrolDirection = 1; // 1 = right, -1 = left

    protected override void OnPatrolSetup()
    {
        patrolCenter = new Vector2(transform.position.x, (patrolY == 0f ? transform.position.y : patrolY));
    }

    protected override void MovePatrol()
    {
        // target X within radius
        float targetX = patrolCenter.x + patrolDirection * patrolRadius;
        float dx = targetX - transform.position.x;

        // wall check ahead
        Vector2 wallOrigin = (Vector2)transform.position;
        RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, new Vector2(patrolDirection, 0f), wallCheckDistance, obstacleLayer);
        bool hasWallAhead = wallHit.collider != null;

        // reverse on wall or if reached edge of radius (close enough)
        if (hasWallAhead || Mathf.Abs(dx) <= reachThreshold)
        {
            patrolDirection *= -1;
            StopHorizontal();
            return;
        }

        // move toward target X while holding patrolCenter.y
        float dirX = Mathf.Sign(dx);
        float vy = (patrolCenter.y - transform.position.y) * verticalSpeedMultiplier;
        rb.linearVelocity = new Vector2(dirX * moveSpeed, vy);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dirX < 0;
    }

    protected override void MoveChase(Vector2 toPlayer)
    {
        Vector2 dir = toPlayer.normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, dir.y * moveSpeed * verticalSpeedMultiplier);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dir.x < 0;
    }

    protected override void MoveSearchToward(Vector2 target)
    {
        Vector2 toTarget = target - (Vector2)transform.position;
        Vector2 dir = toTarget.normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, dir.y * moveSpeed * verticalSpeedMultiplier);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dir.x < 0;
    }

    protected override bool HasReachedTarget(Vector2 target)
    {
        return Vector2.Distance(transform.position, target) <= reachThreshold;
    }

    // For flying enemies we accept checkpoints as-is (no ground projection)
    protected override void BuildCheckpointSnapshot(List<Vector2> dest)
    {
        if (PlayerCheckPoint == null) return;

        for (int i = PlayerCheckPoint.Count - 1; i >= 0; i--)
            dest.Add((Vector2)PlayerCheckPoint[i]);
    }
}