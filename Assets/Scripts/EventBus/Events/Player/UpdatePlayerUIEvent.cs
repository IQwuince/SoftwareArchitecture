using UnityEngine;
using System.Collections;

namespace Assets.Scripts.EventBus.Events
{
	public class UpdatePlayerUIEvent : Event
	{
        public PlayerLevel playerLevel { get; }
        public GameManager gameManager { get; }

        public UpdatePlayerUIEvent(PlayerLevel playerlevel)
        {
            playerLevel = playerLevel;
        }

        public UpdatePlayerUIEvent(GameManager gm)
        {
            gameManager = gm;
        }

    }
}