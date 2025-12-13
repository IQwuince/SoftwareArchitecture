using System.Runtime.CompilerServices;
using UnityEngine;


    public class HealingUsable : MonoBehaviour
{
    [Header("Healing settings")]
    public int healingAmount;

    [Header("References")]
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
    }

    private void Update()
    {

    }

    void HealPlayer(int healingAmount)
    {
        playerHealth.Heal(healingAmount);
    }
}
