using UnityEngine;

public class PlayerHealthUpgradeEvent : Event
{
    public PlayerHealth PlayerHealth { get; }
    public int healthValue;

    public PlayerHealthUpgradeEvent(int healthChange)
    {
        healthValue = healthChange;
    }
}
