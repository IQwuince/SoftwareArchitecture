using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class LevelSystem : MonoBehaviour
{

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, totalExperience;
    int previousLevelExperience, nextLevelExperience;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    private void Start()
    {
        UpdateLevel();
    }


    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            AddExperience(50);
        }
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateUI();
    }

    public void CheckForLevelUp()
    {
        if(totalExperience >= nextLevelExperience)
        {
            currentLevel++;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateUI();
    }

    void UpdateUI()
    {
        int startExperience = totalExperience - previousLevelExperience;
        int endExperience = nextLevelExperience - previousLevelExperience;

        levelText.text = currentLevel.ToString();
        experienceText.text = startExperience + "Exp /" + endExperience + "Exp";
        experienceFill.fillAmount = (float)startExperience / endExperience;
    }
}
