using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SkillTree : MonoBehaviour
{
    [Header("References")]
    private LevelSystem levelSystem;

    [Header("UI Elements")]
    public TextMeshProUGUI skillPointsText;
    public GameObject skillTreeUI;

    private void Awake()
    {
        levelSystem = UnityEngine.Object.FindFirstObjectByType<LevelSystem>();
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            OpenSkillTreeUI();
        }
    }
    private void OpenSkillTreeUI()
    {
        skillTreeUI.SetActive(!skillTreeUI.activeSelf);
    }

}
