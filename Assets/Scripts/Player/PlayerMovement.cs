using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float verticalBounceBackForce = 5f;
    public float horizontalBounceBackForce = 5f;

    [Header("Knockback")]
    [Tooltip("How long the player is knocked back and input is disabled")]
    public float knockbackDuration = 0.25f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    public float moveInput;
    private bool jumpInput;
    public bool isFlipped;

    [Header("Checkpoint Trail")]
    public float checkpointInterval = 0.5f;
    public int maxCheckpoints = 20;
    public List<Vector3> checkpointTrail = new List<Vector3>();
    private float checkpointTimer;

    // Knockback state
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Read movement input (only read; application happens in FixedUpdate)
        moveInput = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ? 1f : 0f;

        // Read jump input
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpInput = true;
        }

        // Checkpoint timer
        checkpointTimer += Time.deltaTime;
        if (checkpointTimer >= checkpointInterval)
        {
            checkpointTimer = 0f;
            AddCheckpoint(transform.position);
        }

    }


    private void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        // Horizontal movement: only apply player input when NOT knocked back
        float targetX = rb.linearVelocity.x;
        if (!isKnockedBack)
        {
            targetX = moveInput * moveSpeed;

            // Flip sprite based on input
            if (moveInput != 0)
            {
                spriteRenderer.flipX = moveInput < 0;
                isFlipped = spriteRenderer.flipX;
            }
        }

        // Apply velocity while preserving current vertical velocity
        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        // Jump
        if (jumpInput && isGrounded && !isKnockedBack)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpInput = false;
        }
        else
        {
            jumpInput = false;
        }
    }

    private void AddCheckpoint(Vector3 pos)
    {
        checkpointTrail.Add(pos);
        if (checkpointTrail.Count > maxCheckpoints)
            checkpointTrail.RemoveAt(0);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnDisable()
    {
       EventBus.UnSubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }


    private void OnPlayerDamaged(PlayerDamagedEvent onPlayerDamagedEvent)
    {
        // Determine horizontal direction based on sprite flip (facing right -> not flipped)
        float kbX = isFlipped ? horizontalBounceBackForce : -horizontalBounceBackForce;

        // Apply immediate velocity for knockback
        rb.linearVelocity = new Vector2(kbX, verticalBounceBackForce);

        // Enter knockback state
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw checkpoint trail
        Gizmos.color = Color.cyan;
        for (int i = 0; i < checkpointTrail.Count; i++)
        {
            Gizmos.DrawSphere(checkpointTrail[i], 0.15f);
            if (i > 0)
                Gizmos.DrawLine(checkpointTrail[i - 1], checkpointTrail[i]);
        }
    }
}