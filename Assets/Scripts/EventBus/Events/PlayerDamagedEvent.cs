using UnityEngine;

public class PlayerDamagedEvent : IGameEvent
{
    public PlayerHealth PlayerHealth { get; }
    public PlayerDamagedEvent(PlayerHealth playerHealth)
    {
        PlayerHealth = playerHealth;
    }
}
