using UnityEngine;
using System.Collections;


public class PlayerDamageUpgradeEvent : IGameEvent
{
	public PlayerCombat PlayerCombat { get; }

    public int damageValue;
    public PlayerDamageUpgradeEvent(int damageChange)
    {
        damageValue = damageChange;
    }

}
