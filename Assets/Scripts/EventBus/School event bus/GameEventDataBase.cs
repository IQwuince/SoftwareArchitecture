using UnityEngine;
// All game events are listed here


/// <summary>
/// Pubished when player got hurt
/// </summary>
public class PlayerHurtEventData : EventData
{
    public int damage;
    public PlayerHurtEventData(int pDamage)
    {
        name = "PlayerHurtEvent";
        damage = pDamage;
    }

    //Overriding ToString method to display event information for debugging
    public override string ToString()
    {
        return "Event name: " + name + "\n" +
                   "Player took " + damage.ToString() + " damage";
    }
}

/// <summary>
/// Published when game over.
/// </summary>
public class GameOverEventData : EventData
{
    public string gameOverText;

    public GameOverEventData(string pGameOverText)
    {
        name = "GameOverEvent";
        gameOverText = pGameOverText;
    }

    //Overriding ToString method to display event information for debugging
    public override string ToString()
    {
        return "Event name: " + name;
    }
}