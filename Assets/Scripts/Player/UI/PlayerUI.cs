using UnityEngine;
using TMPro;
using Assets.Scripts.EventBus.Events;

public class PlayerUI : MonoBehaviour
{
    [Header("text")]
    public TMPro.TextMeshProUGUI playerHealthText;

    private int currentHealthUI;
    private int minHealthUI;
    private int maxHealthUI;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerUIValueChangeEvent>(PlayerHealhValueChange);
        
    }
    private void OnDisable()
    {
        EventBus.UnSubscribe<PlayerUIValueChangeEvent>(PlayerHealhValueChange);
    }


    void PlayerHealhValueChange(PlayerUIValueChangeEvent playerUIChangeEvent)
    {
        currentHealthUI = playerUIChangeEvent.currentHealthE;
        minHealthUI = playerUIChangeEvent.minHealthE;
        maxHealthUI = playerUIChangeEvent.maxHealthE;

        UpdatePlayerUI();
    }

     void UpdatePlayerUIEvent(UpdatePlayerUIEvent updatePlayerUIEvent)
    {
        UpdatePlayerUI();
    }

    void UpdatePlayerUI()
    {
        if (playerHealthText != null) playerHealthText.text = currentHealthUI.ToString() + " / " + maxHealthUI.ToString();
    }

}
