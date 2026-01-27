using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemyPlayerTrailCheckPointEvent : IGameEvent
{
    public EnemyMovement2D EnemyMovement { get; }
    public List<Vector2> CheckpointTrailT;

    public EnemyPlayerTrailCheckPointEvent(List<Vector2> checkpointTrailE)
    {
        CheckpointTrailT = checkpointTrailE;
    }
}
