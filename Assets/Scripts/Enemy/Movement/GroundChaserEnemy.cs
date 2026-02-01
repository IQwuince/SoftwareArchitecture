using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundChaserEnemy : EnemyMovement2D
{
    [Header("Ground Patrol Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float frontOffset = 0.4f;       
    [SerializeField] private float groundCheckDistance = 0.8f;
    [SerializeField] private float wallCheckDistance = 0.35f;
    [SerializeField] private float flipCooldown = 0.12f;

    private int patrolDirection = 1; // 1 = right, -1 = left
    private float lastFlipTime;

    protected override void OnPatrolSetup()
    {
        // keep current facing direction 
    }

    protected override void MovePatrol()
    {
        Vector2 feetOffset = new Vector2(0f, -0.4f);
        Vector2 frontOrigin = (Vector2)transform.position + new Vector2(patrolDirection * frontOffset, feetOffset.y);

        // ground ahead?
        RaycastHit2D groundHit = Physics2D.Raycast(frontOrigin, Vector2.down, groundCheckDistance, groundLayer);
        bool hasGroundAhead = groundHit.collider != null;

        // wall ahead?
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(patrolDirection * frontOffset, 0f);
        RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, new Vector2(patrolDirection, 0f), wallCheckDistance, obstacleLayer);
        bool hasWallAhead = wallHit.collider != null;

        if ((!hasGroundAhead || hasWallAhead) && Time.time - lastFlipTime >= flipCooldown)
        {
            FlipDirection();
            return;
        }

        // move horizontally
        float vx = patrolDirection * moveSpeed;
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

        if (spriteRenderer != null)
            spriteRenderer.flipX = patrolDirection < 0;
    }

    private void FlipDirection()
    {
        patrolDirection *= -1;
        lastFlipTime = Time.time;
    }

    // Flip only on solid enemy collision (ignore triggers)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.isTrigger) return;

        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            if (Time.time - lastFlipTime >= flipCooldown)
                FlipDirection();
        }
    }

    protected override void MoveChase(Vector2 toPlayer)
    {
        float dirX = Mathf.Sign(toPlayer.x);
        if (Mathf.Approximately(dirX, 0f))
            dirX = toPlayer.x >= 0f ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dirX < 0;
    }

    protected override void MoveSearchToward(Vector2 target)
    {
        float dx = target.x - transform.position.x;
        float dirX = Mathf.Sign(dx);
        if (Mathf.Approximately(dirX, 0f) && Mathf.Abs(dx) > 0.01f)
            dirX = dx > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dirX < 0;
    }

    protected override bool HasReachedTarget(Vector2 target)
    {
        return Mathf.Abs(target.x - transform.position.x) <= reachThreshold;
    }

    protected override void BuildCheckpointSnapshot(List<Vector2> dest)
    {
        if (PlayerCheckPoint == null)
            return;

        for (int i = PlayerCheckPoint.Count - 1; i >= 0; i--)
        {
            Vector3 cp = PlayerCheckPoint[i];

            if (cp.y - transform.position.y > 0.6f) continue;

            Vector2 start = new Vector2(cp.x, cp.y + 0.1f);
            RaycastHit2D groundHit = Physics2D.Raycast(start, Vector2.down, 6f, groundLayer);
            if (groundHit.collider != null)
                dest.Add(groundHit.point);
            else
                dest.Add(new Vector2(cp.x, transform.position.y));
        }
    }
}