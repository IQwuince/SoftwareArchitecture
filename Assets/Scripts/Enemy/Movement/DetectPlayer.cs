using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public Transform detectPlayerPosition;
    public EnemyCombat enemyCombat;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("player detected");
            detectPlayerPosition = collision.transform;
            //EventBus<EnemyInPlayerReachEvent>.Publish(new EnemyInPlayerReachEvent(true));
            enemyCombat.playerInReach = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //EventBus<EnemyInPlayerReachEvent>.Publish(new EnemyInPlayerReachEvent(false));
            enemyCombat.playerInReach = false;
        }
    }
}
