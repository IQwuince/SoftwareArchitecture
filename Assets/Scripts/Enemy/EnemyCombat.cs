using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyCombat : MonoBehaviour
{
    public int damageAmount;

    public GameObject bullet;
    public Transform bulletPos;
    private bool playerInReach;

    private float reloadTimer;
    [SerializeField] protected float bulletDuration;
    [SerializeField] private float fireInterval;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //EventBus.Publish(new PlayerDamagedEvent(damageAmount));
            
        }
    }
    private void OnEnable()
    {
        EventBus.Subscribe<EnemyInPlayerReachEvent>(isPlayerInReach);
    }
    private void OnDisable()
    {
        EventBus.UnSubscribe<EnemyInPlayerReachEvent>(isPlayerInReach);
    }
    void isPlayerInReach(EnemyInPlayerReachEvent EnemyInPlayerReachEvent)
    {
        playerInReach = EnemyInPlayerReachEvent.isPlayerInReach;
    }
    private void Update()
    {

        if ( bullet == null || bulletPos == null) return;

        reloadTimer += Time.deltaTime;

        if (reloadTimer > fireInterval && playerInReach == true)
        {   
            reloadTimer = 0;
            Shoot();
        }
    }

    protected virtual void Shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }
}