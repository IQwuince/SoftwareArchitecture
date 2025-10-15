using UnityEngine;
using TMPro;

public class PlayerHealth : GenericHealth
{
    public TMPro.TextMeshProUGUI healthText;

    private void Start()
    {
        
    }

    private void Update()
    {
        healthText.text = currentHealth.ToString();
    }
}

