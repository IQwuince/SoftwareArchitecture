using UnityEngine;

public class EnemyKilledEvent : Event
{
    public readonly GameObject enemyPrefab;

    public EnemyKilledEvent(GameObject pEnemyPrefab)
    {
        enemyPrefab = pEnemyPrefab;
    }
}
