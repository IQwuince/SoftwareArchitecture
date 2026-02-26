using UnityEngine;

public class EnemyBullletScript : MonoBehaviour
{
    [SerializeField] private int bulletDamage;
    [SerializeField] private float lifetime;
    [SerializeField] private float speed;

    private Vector2 direction;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        direction = (player.transform.position - transform.position).normalized;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            Destroy(gameObject);


        if (collision.CompareTag("Player"))
        {
            EventBus<PlayerDamagedEvent>.Publish(new PlayerDamagedEvent(bulletDamage));
            Destroy(gameObject);
        }
    }
}