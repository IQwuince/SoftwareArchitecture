using UnityEngine;

public class EnemyBullletScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;

    [SerializeField] private int bulletDamage;
    public float lifetime;
    public float force;

    private void Awake()
    {
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (rb != null)
        {
            rb.gravityScale = 0f; // no drop
        }

        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * force;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            EventBus.Publish(new PlayerDamagedEvent(bulletDamage));
        }

        Debug.Log("bulletHit");
    }
}