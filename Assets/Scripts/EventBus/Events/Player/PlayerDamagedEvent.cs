using UnityEngine;

public class PlayerDamagedEvent : IGameEvent
{
    public PlayerHealth PlayerHealth { get; }
    public int damageTaken;
    public PlayerDamagedEvent(int damageE)
    {
        damageTaken = damageE;


    }
}
