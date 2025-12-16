using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public abstract class EnemyMovement2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected EnemyHealth enemyHealth;
    [SerializeField] protected Transform player;
    public TextMeshProUGUI stateText;

    protected PlayerMovement playerMovement;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    [Header("General Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected LayerMask obstacleLayer;  // blockers only (no player)
    [SerializeField] protected Vector2 eyeOffset = new Vector2(0f, 0.25f);
    [SerializeField] protected float raycastInterval = 0.12f;

    [Header("Search Settings")]
    [SerializeField] protected float reachThreshold = 0.25f;
    [SerializeField] protected float searchTimeout = 8f;

    protected enum EnemyState { Idle, Patrol, Chase, Search }
    protected EnemyState currentState = EnemyState.Patrol;

    // LOS
    protected float lastRaycastTime;
    protected Vector2 lastSeenPlayerPos;

    // Search
    protected readonly List<Vector2> lastKnownCheckpoints = new List<Vector2>();
    protected int searchIndex;
    protected float searchStartTime;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        // Find the player by tag at runtime
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                // Use the parent transform if it exists, otherwise use the player's own transform
                player = playerObj.transform.parent != null ? playerObj.transform.parent : playerObj.transform;
            }
        }

        // Try to get PlayerMovement robustly
        if (player != null)
        {
            playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement == null)
                playerMovement = player.GetComponent<PlayerMovement>();
        }

        // final fallback: locate any PlayerMovement in the scene
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();

        OnPatrolSetup();
    }

    protected virtual void Update()
    {
        if (player == null || enemyHealth == null || enemyHealth.currentHealth <= 0) return;

        if (Time.time - lastRaycastTime >= raycastInterval)
        {
            lastRaycastTime = Time.time;
            HandleLineOfSight();
        }

        UpdateUIState();
    }

    protected virtual void FixedUpdate()
    {
        if (enemyHealth == null || enemyHealth.currentHealth <= 0)
        {
            StopHorizontal();
            return;
        }

        switch (currentState)
        {
            case EnemyState.Chase:
                TickChase();
                break;
            case EnemyState.Search:
                TickSearch();
                break;
            case EnemyState.Patrol:
                TickPatrol();
                break;
            case EnemyState.Idle:
            default:
                TickIdle();
                break;
        }
    }

    public void UpdateUIState()
    {
        if (stateText != null)
        {
            stateText.text = "State: " + currentState.ToString();
        }
    }

    // State machine helpers
    protected void EnterState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (newState == EnemyState.Idle)
            StopHorizontal();

        if (newState == EnemyState.Search)
        {
            lastKnownCheckpoints.Clear();
            BuildCheckpointSnapshot(lastKnownCheckpoints);
            searchIndex = 0;
            searchStartTime = Time.time;

            // If snapshot produced nothing, try to seed with last seen player position
            if (lastKnownCheckpoints.Count == 0 && lastSeenPlayerPos != Vector2.zero)
            {
                lastKnownCheckpoints.Add(lastSeenPlayerPos);
            }
        }

        if (newState == EnemyState.Patrol)
            OnPatrolSetup();
    }

    protected virtual void TickIdle() => StopHorizontal();

    protected virtual void TickChase()
    {
        if (player == null) return;
        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        MoveChase(toPlayer);

        // if actively chasing, update last seen pos
        lastSeenPlayerPos = player.position;
    }

    protected virtual void TickSearch()
    {
        if (lastKnownCheckpoints.Count == 0)
        {
            if (Time.time - searchStartTime > searchTimeout) EnterState(EnemyState.Patrol);
            else StopHorizontal();
            return;
        }

        if (searchIndex >= lastKnownCheckpoints.Count)
        {
            EnterState(EnemyState.Patrol);
            return;
        }

        Vector2 target = lastKnownCheckpoints[searchIndex];

        if (HasReachedTarget(target))
        {
            searchIndex++;
            // small pause when reaching a checkpoint
            StopHorizontal();
            return;
        }

        MoveSearchToward(target);

        if (Time.time - searchStartTime > searchTimeout)
            EnterState(EnemyState.Patrol);
    }

    protected virtual void TickPatrol()
    {
        // Patrol-specific behavior implemented by derived classes
        MovePatrol();
    }

    // LOS
    protected void HandleLineOfSight()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return;

        Vector2 origin = GetEyeOrigin();
        Vector2 dir = ((Vector2)player.position - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distanceToPlayer, obstacleLayer);
        bool blocked = hit.collider != null;

        if (!blocked)
        {
            lastSeenPlayerPos = player.position;
            EnterState(EnemyState.Chase);
        }
        else
        {
            if (currentState == EnemyState.Chase)
                EnterState(EnemyState.Search);
        }
    }

    protected virtual Vector2 GetEyeOrigin()
    {
        return (Vector2)transform.position + eyeOffset;
    }

    // Helpers
    protected void StopHorizontal()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    // Snapshot building (default newest-first copy)
    // This base implementation copies the player's checkpointTrail as-is (newest first).
    // Derived classes may override to filter or project to ground.
    protected virtual void BuildCheckpointSnapshot(List<Vector2> dest)
    {
        // ensure we have a valid PlayerMovement reference
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null || playerMovement.checkpointTrail == null) return;
        }

        var trail = playerMovement.checkpointTrail;
        for (int i = trail.Count - 1; i >= 0; i--)
        {
            dest.Add((Vector2)trail[i]);
        }
    }

    // Abstract movement hooks derived classes must implement
    protected abstract void MoveChase(Vector2 toPlayer);
    protected abstract void MoveSearchToward(Vector2 target);
    protected abstract bool HasReachedTarget(Vector2 target);

    // Patrol hooks
    protected abstract void OnPatrolSetup();
    protected abstract void MovePatrol();

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.position;
        float distance = Vector3.Distance(enemyPos, playerPos);
        Vector3 direction = (playerPos - enemyPos).normalized;
        Vector3 rangeEnd = enemyPos + direction * detectionRange;

        // Raycast to check if player is in sight
        RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, distance, obstacleLayer);
        bool canSeePlayer = (hit.collider == null);

        if (canSeePlayer)
        {
            // Always draw green up to detection range
            Gizmos.color = Color.green;
            Gizmos.DrawLine(enemyPos, distance > detectionRange ? rangeEnd : playerPos);

            // If player is further than detection range, draw red from range end to player
            if (distance > detectionRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(rangeEnd, playerPos);
            }
        }
        else
        {
            // Out of sight: whole line is red
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemyPos, playerPos);
        }
    }
}
