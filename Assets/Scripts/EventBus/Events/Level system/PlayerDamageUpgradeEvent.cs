using UnityEngine;
using System.Collections;


public class PlayerDamageUpgradeEvent : Event
{
	public PlayerCombat PlayerCombat { get; }

    public int damageValue;
    public PlayerDamageUpgradeEvent(int damageChange)
    {
        damageValue = damageChange;
    }

}
