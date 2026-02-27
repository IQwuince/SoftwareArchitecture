using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Assets.Scripts.Enemy.Movement;

public abstract class EnemyMovement2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected EnemyHealth enemyHealth;
    [SerializeField] protected DetectPlayer detectPlayer;
    [SerializeField] private TextMeshPro stateText;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    [Header("General Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected Vector2 eyeOffset = new(0f, 0.25f);
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

    protected float lastRaycastTime;
    protected Vector2 lastSeenPlayerPos;

    protected readonly List<Vector2> lastKnownCheckpoints = new();
    protected int searchIndex;
    protected float searchStartTime;

    protected List<Vector3> PlayerCheckPoint = new();

    private EnemyFSM stateMachine;
    private EnemyIdleState idleState;
    private EnemyPatrolState patrolState;
    private EnemyChaseState chaseState;
    private EnemySearchState searchState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        BuildFSM();
    }

    private void OnEnable()
    {
        EventBus<EnemyPlayerTrailCheckPointEvent>.OnEvent += OnPlayerConnected;
        EventBus<EnemyDamagedEvent>.OnEvent += EnemyDamaged;
    }

    private void OnDisable()
    {
        EventBus<EnemyPlayerTrailCheckPointEvent>.OnEvent -= OnPlayerConnected;
        EventBus<EnemyDamagedEvent>.OnEvent -= EnemyDamaged;
    }

    protected virtual void Update()
    {
        if (!IsAlive()) return;

        if (Time.time - lastRaycastTime >= raycastInterval)
        {
            lastRaycastTime = Time.time;
            HandleLineOfSight();
        }

        stateMachine?.Tick();
        UpdateUIState();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsAlive())
        {
            StopHorizontal();
            return;
        }

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
            else return;
        }

        stateMachine?.PhysicsTick();
    }

    private void BuildFSM()
    {
        stateMachine = new EnemyFSM();

        idleState = new EnemyIdleState(this);
        patrolState = new EnemyPatrolState(this);
        chaseState = new EnemyChaseState(this);
        searchState = new EnemySearchState(this);

        idleState.AddTransition(() => CanSeePlayer(), chaseState);
        idleState.AddTransition(() => !CanSeePlayer(), patrolState);

        patrolState.AddTransition(() => CanSeePlayer(), chaseState);

        chaseState.AddTransition(() => !CanSeePlayer(), searchState);

        searchState.AddTransition(() => CanSeePlayer(), chaseState);
        searchState.AddTransition(() => IsSearchFinishedOrTimedOut(), patrolState);

        stateMachine.SetInitialState(patrolState);
    }

    public void UpdateUIState()
    {
        if (stateText != null)
            stateText.text = "State: " + (stateMachine != null ? stateMachine.CurrentStateName : "None");
    }

    protected bool IsAlive()
    {
        return detectPlayer != null && detectPlayer.detectPlayerPosition != null &&
               enemyHealth != null && enemyHealth.currentHealth > 0;
    }

    protected virtual Vector2 GetEyeOrigin() => (Vector2)transform.position + eyeOffset;

    protected bool CanSeePlayer()
    {
        if (detectPlayer == null || detectPlayer.detectPlayerPosition == null) return false;

        float distance = Vector2.Distance(transform.position, detectPlayer.detectPlayerPosition.position);
        if (distance > detectionRange) return false;

        Vector2 origin = GetEyeOrigin();
        Vector2 dir = ((Vector2)detectPlayer.detectPlayerPosition.position - origin).normalized;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, obstacleLayer);
        bool blocked = hit.collider != null;

        if (!blocked)
            lastSeenPlayerPos = detectPlayer.detectPlayerPosition.position;

        return !blocked;
    }

    protected void HandleLineOfSight()
    {
        // kept for compatibility / debugging; transitions use CanSeePlayer()
        _ = CanSeePlayer();
    }

    protected bool IsSearchFinishedOrTimedOut()
    {
        if (Time.time - searchStartTime > searchTimeout) return true;
        return searchIndex >= lastKnownCheckpoints.Count && lastKnownCheckpoints.Count > 0;
    }

    public bool IsSearchTimedOut() => Time.time - searchStartTime > searchTimeout;

    public void PrepareSearchSnapshot()
    {
        lastKnownCheckpoints.Clear();
        BuildCheckpointSnapshot(lastKnownCheckpoints);
        searchIndex = 0;
        searchStartTime = Time.time;

        if (lastKnownCheckpoints.Count == 0 && lastSeenPlayerPos != Vector2.zero)
            lastKnownCheckpoints.Add(lastSeenPlayerPos);
    }

    protected void StopHorizontal()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    // wrappers for state classes
    public void StopHorizontalPublic() => StopHorizontal();
    public void OnPatrolSetupPublic() => OnPatrolSetup();
    public void MovePatrolPublic() => MovePatrol();
    public void MoveChasePublic(Vector2 toPlayer) => MoveChase(toPlayer);
    public void MoveSearchTowardPublic(Vector2 target) => MoveSearchToward(target);
    public bool HasReachedTargetPublic(Vector2 target) => HasReachedTarget(target);

    public bool HasPlayerTarget() => detectPlayer != null && detectPlayer.detectPlayerPosition != null;
    public Vector2 GetToPlayerVector() => (Vector2)detectPlayer.detectPlayerPosition.position - (Vector2)transform.position;
    public void UpdateLastSeenPlayerPosFromTarget()
    {
        if (detectPlayer != null && detectPlayer.detectPlayerPosition != null)
            lastSeenPlayerPos = detectPlayer.detectPlayerPosition.position;
    }

    public int SearchPointsCount => lastKnownCheckpoints.Count;
    public int SearchIndex => searchIndex;
    public Vector2 GetSearchPointAt(int index) => lastKnownCheckpoints[index];
    public void AdvanceSearchIndex() => searchIndex++;

    protected virtual void BuildCheckpointSnapshot(List<Vector2> dest) { }

    protected abstract void MoveChase(Vector2 toPlayer);
    protected abstract void MoveSearchToward(Vector2 target);
    protected abstract bool HasReachedTarget(Vector2 target);
    protected abstract void OnPatrolSetup();
    protected abstract void MovePatrol();

    private void OnPlayerConnected(EnemyPlayerTrailCheckPointEvent e)
    {
        PlayerCheckPoint = e.CheckpointTrailT.ConvertAll(v2 => new Vector3(v2.x, v2.y, 0f));
    }

    public void KnockBackEnemy()
    {
        Vector2 direction = Vector2.up;

        if (detectPlayer != null && detectPlayer.detectPlayerPosition != null)
        {
            float xDir = (transform.position.x - detectPlayer.detectPlayerPosition.position.x) >= 0 ? 1f : -1f;
            direction = new Vector2(xDir, 1f);
        }

        rb.linearVelocity = new Vector2(direction.x * horizontalBounceBackForce, direction.y * verticalBounceBackForce);
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }

    void EnemyDamaged(EnemyDamagedEvent e)
    {
        KnockBackEnemy();
    }
}