using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public Transform detectPlayerPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detectPlayerPosition = collision.transform;
            EventBus.Publish(new EnemyInPlayerReachEvent(true));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventBus.Publish(new EnemyInPlayerReachEvent(false));
        }
    }
}
