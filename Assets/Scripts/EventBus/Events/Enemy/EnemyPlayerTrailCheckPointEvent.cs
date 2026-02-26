using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerTrailCheckPointEvent : Event
{
    public EnemyMovement2D EnemyMovement { get; }
    public List<Vector2> CheckpointTrailT;

    public EnemyPlayerTrailCheckPointEvent(List<Vector2> checkpointTrailE)
    {
        CheckpointTrailT = checkpointTrailE;
    }
}
