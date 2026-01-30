using TreeEditor;
using UnityEngine;

public class EnemyBullletScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;

    [SerializeField] private int bulletDamage;
    public float lifetime;

    public float force;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }

    private void Awake()
    {
        Destroy(gameObject, lifetime);
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
    }
}
