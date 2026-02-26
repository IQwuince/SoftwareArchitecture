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
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private Image experienceFill;
    [SerializeField] private TextMeshProUGUI levelPointsText;

    [Header("Levels")]
    public int levelPoints;

    private void Start()
    {
        UpdateLevel(); 
        UpdateUI();
    }
    private void OnEnable()
    {
        EventBus<LevelSystemAddXpEvent>.OnEvent += AddXpEvent;
    }
    private void OnDisable()
    {
        EventBus<LevelSystemAddXpEvent>.OnEvent -= AddXpEvent;
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

    void AddXpEvent(LevelSystemAddXpEvent levelSystemAddXpEvent)
    {
        AddExperience(levelSystemAddXpEvent.XpAdd);
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

    public void UpdateUI()
    {
        int startExperience = totalExperience - previousLevelExperience;
        int endExperience = nextLevelExperience - previousLevelExperience;
        if(levelText != null)levelText.text = currentLevel.ToString();
        if (levelPointsText != null) levelPointsText.text = levelPoints.ToString();
        if (experienceText != null) experienceText.text = $"{startExperience} Exp / {endExperience} Exp";
        if ( experienceFill != null) experienceFill.fillAmount = endExperience > 0 ? (float)startExperience / endExperience : 0f;
    }

    void SpendLevel(int points)
    {
        if (levelPoints >= points)
        {
            levelPoints -= points;
        }
    }


}