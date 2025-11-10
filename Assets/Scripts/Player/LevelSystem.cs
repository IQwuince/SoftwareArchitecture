using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class LevelSystem : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    public int currentLevel = 0;
    int totalExperience = 0;
    int previousLevelExperience = 0;
    int nextLevelExperience = 1;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;
    [SerializeField] TextMeshProUGUI levelPointsText;

    [Header("Levels")]
    public int levelPoints;

    private void Start()
    {
        UpdateLevel(); 
        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            AddExperience(50);
        }

        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            SpendLevel(1);
            UpdateUI();
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
        while (totalExperience >= nextLevelExperience)
        {
            currentLevel++;
            levelPoints++;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        if (nextLevelExperience <= previousLevelExperience) nextLevelExperience = previousLevelExperience + 1;
    }

    void UpdateUI()
    {
        int startExperience = totalExperience - previousLevelExperience;
        int endExperience = nextLevelExperience - previousLevelExperience;
        levelText.text = currentLevel.ToString();
        levelPointsText.text = levelPoints.ToString();
        experienceText.text = $"{startExperience} Exp / {endExperience} Exp";
        experienceFill.fillAmount = endExperience > 0 ? (float)startExperience / endExperience : 0f;
    }

    void SpendLevel(int points)
    {
        if (levelPoints >= points)
        {
            levelPoints -= points;
        }
    }
}