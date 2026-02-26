using UnityEngine;

public class GE_PlayerDamaged : GameEvent
{
    public PlayerHealth PlayerHealth { get; }
    public int damageTaken;
    public GE_PlayerDamaged(int damageE)
    {
        damageTaken = damageE;


    }
}
