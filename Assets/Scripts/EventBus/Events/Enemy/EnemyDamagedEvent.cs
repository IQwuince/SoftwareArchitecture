using UnityEngine;
using System.Collections;


public class EnemyDamagedEvent : IGameEvent
{
	public EnemyHealth EnemyHealthE { get; }
	public PlayerCombat playerCombatE { get; }

	public EnemyDamagedEvent(PlayerCombat playerCombat)
	{
		playerCombatE = playerCombat;

    }

	public EnemyDamagedEvent(EnemyHealth enemyHealth)
	{
        EnemyHealthE = enemyHealth;

    }

}
