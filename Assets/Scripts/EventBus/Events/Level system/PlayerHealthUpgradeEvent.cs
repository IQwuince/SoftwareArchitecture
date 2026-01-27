using UnityEngine;

public class PlayerHealthUpgradeEvent : IGameEvent
{
    public PlayerHealth PlayerHealth { get; }
    public int healthValue;

    public PlayerHealthUpgradeEvent(int healthChange)
    {
        healthValue = healthChange;
    }
}
