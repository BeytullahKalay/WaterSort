using System.Collections.Generic;
using UnityEngine;

namespace LevelScripts
{
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
            AssignValues(_levelValue,levelMaker.LevelMakerStateController);
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

        private void AssignValues(LevelMakerValue assignValue,LevelMakerStateController levelMakerStateController)
        {
            levelMaker.SetNumberOfColorToCreate(assignValue.ColorAmount);
            levelMakerStateController.NoMatches = assignValue.NoMatches;
            levelMakerStateController.RainbowBottle = assignValue.RainbowBottle;
        }
    }
}