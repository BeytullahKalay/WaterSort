using System.Collections.Generic;
using LevelScripts;
using UnityEngine;

public class LevelMakerController : MonoBehaviour
{
    [SerializeField] private LevelMaker levelMaker;

    [Space(20)] [SerializeField]
    private List<LevelMakerValue> levelMakerControllers = new List<LevelMakerValue>();

    private int _levelIndex;

    private LevelMakerValue _levelValue;

    private void OnEnable()
    {
        EventManager.LevelCompleted += FindAndAssignValues;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= FindAndAssignValues;
    }

    private void Awake()
    {
        FindAndAssignValues();
    }

    private void FindAndAssignValues()
    {
        _levelValue = FindLevelMakerValue();
        AssignValues(_levelValue);
    }

    private LevelMakerValue FindLevelMakerValue()
    {
        _levelIndex = PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex);
        foreach (var levelMakerController in levelMakerControllers)
        {
            if (_levelIndex >= levelMakerController.LevelBeginningIndex &&
                _levelIndex <= levelMakerController.LevelFinishIndex)
            {
                return levelMakerController;
            }
        }

        LevelMakerValue finalValue = new LevelMakerValue
        {
            ColorAmount = 12,
            NoMatches = true,
            RainbowBottle = true,
        };

        return finalValue;
    }

    private void AssignValues(LevelMakerValue assignValue)
    {
        levelMaker.NumberOfColorsToCreate = assignValue.ColorAmount;
        levelMaker.NoMatches = assignValue.NoMatches;
        levelMaker.RainbowBottle = assignValue.RainbowBottle;
    }
    
}

[System.Serializable]
public class LevelMakerValue
{
    public int LevelBeginningIndex;
    public int LevelFinishIndex;
    [Range(0, 12)] public int ColorAmount;
    public bool NoMatches;
    public bool RainbowBottle;
}