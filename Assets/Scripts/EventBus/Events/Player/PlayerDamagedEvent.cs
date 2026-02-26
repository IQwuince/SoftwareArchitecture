using UnityEngine;

public class PlayerDamagedEvent : Event
{
    public PlayerHealth PlayerHealth { get; }
    public int damageTaken;
    public PlayerDamagedEvent(int damageE)
    {
        damageTaken = damageE;


    }
}
