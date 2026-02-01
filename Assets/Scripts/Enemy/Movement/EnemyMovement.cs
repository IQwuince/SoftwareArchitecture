using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class EnemyMovement2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected EnemyHealth enemyHealth;
    [SerializeField] protected DetectPlayer detectPlayer;
    //protected Transform playerPosition;
    [SerializeField] TextMeshPro stateText;

    //protected PlayerMovement playerMovement;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    [Header("General Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected LayerMask obstacleLayer;  // blockers only (no player)
    [SerializeField] protected Vector2 eyeOffset = new Vector2(0f, 0.25f);
    [SerializeField] protected float raycastInterval = 0.12f;

    [Header("Bounce back")]
    public float verticalBounceBackForce = 5f;
    public float horizontalBounceBackForce = 5f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    public float knockbackDuration = 0.25f;

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

    //Player trail
    protected List<Vector2> PlayerCheckPoint = new();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        OnPatrolSetup();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyPlayerTrailCheckPointEvent>(OnPlayerConnected);
        EventBus.Subscribe<EnemyDamagedEvent>(EnemyDamaged);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<EnemyPlayerTrailCheckPointEvent>(OnPlayerConnected);
        EventBus.UnSubscribe<EnemyDamagedEvent>(EnemyDamaged);
    }

   /* private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPosition = collision.transform;
            EventBus.Publish(new EnemyInPlayerReachEvent(true));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventBus.Publish(new EnemyInPlayerReachEvent(false));
        }
    }*/

    protected virtual void Update()
    {
        if (detectPlayer.detectPlayerPosition == null || enemyHealth == null || enemyHealth.currentHealth <= 0) return;

        if (Time.time - lastRaycastTime >= raycastInterval)
        {
            lastRaycastTime = Time.time;
            HandleLineOfSight();
        }

        UpdateUIState();

        // NOTE: knockback timer is handled in FixedUpdate to align with physics updates
    }

    protected virtual void FixedUpdate()
    {
        if (enemyHealth == null || enemyHealth.currentHealth <= 0)
        {
            StopHorizontal();
            return;
        }

        // Handle knockback in FixedUpdate so physics isn't immediately overwritten by AI movement
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                // Optionally clear horizontal velocity when knockback ends, or let AI resume naturally
                // rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            else
            {
                // Still in knockback; skip AI-driven movement so velocity isn't clobbered.
                return;
            }
        }

        switch (currentState)
        {
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Search:
                Search();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Idle:
            default:
                Idle();
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

    protected virtual void Idle() => StopHorizontal();

    protected virtual void Chase()
    {
        if (detectPlayer.detectPlayerPosition == null) return;
        Vector2 toPlayer = (Vector2)detectPlayer.detectPlayerPosition.position - (Vector2)transform.position;
        MoveChase(toPlayer);

        // if actively chasing, update last seen pos
        lastSeenPlayerPos = detectPlayer.detectPlayerPosition.position;
    }

    protected virtual void Search()
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

    protected virtual void Patrol()
    {
        // Patrol-specific behavior implemented by derived classes
        MovePatrol();
    }

    // LOS
    protected void HandleLineOfSight()
    {
        if (detectPlayer.detectPlayerPosition == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, detectPlayer.detectPlayerPosition.position);
        if (distanceToPlayer > detectionRange) return;

        Vector2 origin = GetEyeOrigin();
        Vector2 dir = ((Vector2)detectPlayer.detectPlayerPosition.position - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distanceToPlayer, obstacleLayer);
        bool blocked = hit.collider != null;

        if (!blocked)
        {
            lastSeenPlayerPos = detectPlayer.detectPlayerPosition.position;
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
        /*// ensure we have a valid PlayerMovement reference
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null || playerMovement.checkpointTrail == null) return;
        }

        var trail = playerMovement.checkpointTrail;
        for (int i = trail.Count - 1; i >= 0; i--)
        {
            dest.Add((Vector2)trail[i]);
        }*/
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
        if (detectPlayer.detectPlayerPosition == null) return;

        Vector3 enemyPos = transform.position;
        Vector3 playerPos = detectPlayer.detectPlayerPosition.position;
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

    private void OnPlayerConnected(EnemyPlayerTrailCheckPointEvent enemyPlayerTrailCheckPointEvent)
    {
        PlayerCheckPoint = enemyPlayerTrailCheckPointEvent.CheckpointTrailT;
    }

    public void KnockBackEnemy()
    {
        Vector2 direction = Vector2.up; // Default: just up

        if (detectPlayer.detectPlayerPosition != null)
        {
            // Knock away from the player horizontally, always up vertically
            float xDir = (transform.position.x - detectPlayer.detectPlayerPosition.position.x) >= 0 ? 1f : -1f;
            // apply horizontal and vertical separately (no normalization) so both forces are effective
            direction = new Vector2(xDir, 1f);
        }

        // Apply immediate physics via velocity so movement doesn't fight against it
        Vector2 knockVelocity = new Vector2(direction.x * horizontalBounceBackForce, direction.y * verticalBounceBackForce);
        rb.linearVelocity = knockVelocity;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }

    void EnemyDamaged(EnemyDamagedEvent enemyDamagedEvent)
    {
        //KnockBackEnemy(); // currently commented out; EnemyHealth triggers KnockBackEnemy directly
    }
}