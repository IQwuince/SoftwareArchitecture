using UnityEngine;
using System.Collections;


public class EnemyInPlayerReachEvent : Event
{
    public EnemyMovement2D enemyMovement2D;
    public bool isPlayerInReach;
    
    public EnemyInPlayerReachEvent(bool isPlayerInReachE)
    {
        isPlayerInReach = isPlayerInReachE;
    }
}
