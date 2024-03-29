using System.Threading;
using DataRepo;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    [RequireComponent(typeof(LevelColorController))]
    [RequireComponent(typeof(CreateBottlesForLevel))]
    [RequireComponent(typeof(BottleCreateBottleState))]
    [RequireComponent(typeof(LevelMakerMainThreadActions))]
    public class LevelMaker : MonoBehaviour
    {
        public Data Data;


        private int _createdBottles;
        private int _numberOfBottlesCreate;
        private int _totalWaterCount;

        private Thread _myThread;

        private LevelColorController _colorController;
        private CreateBottlesForLevel _createBottlesForLevel;
        private BottleCreateBottleState _bottleCreateBottleState;
        private LevelMakerMainThreadActions _levelMakerMainThreadActions;

        public BottleCreateBottleState BottleCreateBottleState { get; private set; }
        public LevelColorController LevelColorController { get; private set; }


        private void Awake()
        {
            _colorController = GetComponent<LevelColorController>();
            _createBottlesForLevel = GetComponent<CreateBottlesForLevel>();
            _bottleCreateBottleState = GetComponent<BottleCreateBottleState>();
            _levelMakerMainThreadActions = GetComponent<LevelMakerMainThreadActions>();

            BottleCreateBottleState = _bottleCreateBottleState;
            LevelColorController = _colorController;

            JsonManager.TryGetLevelCreateDataFromJson(ref Data);
            
            CheckNamingIndexPlayerPref();
        }

        private void OnEnable()
        {
            EventManager.CreateLevel += CreateNewLevel_GUIButton;
            EventManager.CreatePrototype += CreateLevelFromPrototype;
        }

        private void OnDisable()
        {
            EventManager.CreateLevel -= CreateNewLevel_GUIButton;
            EventManager.CreatePrototype -= CreateLevelFromPrototype;
        }

        private void CheckNamingIndexPlayerPref()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefNames.NamingIndex))
            {
                PlayerPrefs.SetInt(PlayerPrefNames.NamingIndex, 0);
            }
        }
        

        // using by inspector gui
        public void CreateNewLevel_GUIButton()
        {
            if (_myThread == null || !_myThread.IsAlive)
                _myThread = new Thread(CreateLevelPrototype);

            _myThread.Start();
        }

        private void CreateLevelPrototype()
        {
            Data.CreatedBottles.Clear();
            _createdBottles = 0;
            _colorController.SelectedColors.Clear();

            _colorController.SelectColorsToCreate(Data);

            _colorController.CreateColorObjects();

            _totalWaterCount = _colorController.SelectedColors.Count * 4;

            _numberOfBottlesCreate = LevelMakerHelper.RandomizeNumberOfBottle(Data, _colorController);

            _createBottlesForLevel.CreateBottles(_numberOfBottlesCreate, _bottleCreateBottleState.NoMatches,
                _bottleCreateBottleState.RainbowBottle, ref _totalWaterCount,
                _colorController, Data, _createdBottles,  _levelMakerMainThreadActions.MainThread_SetBottlePosition);

            var allBottles = new AllBottles(Data.CreatedBottles);
            ColorNumerator.NumerateColors(_colorController.SelectedColors);

            if (allBottles.IsSolvable())
            {
                Debug.Log("Solvable");

                allBottles.NumberOfColorInLevel = _colorController.NumberOfColorsToCreate;

                _levelMakerMainThreadActions.MainThread_SaveToJson(allBottles);

                _levelMakerMainThreadActions.MainThread_SaveLevelCreateDataToJson();
            }
            else
            {
                CreateLevelPrototype();
            }
        }

        private void CreateLevelFromPrototype(AllBottles prototypeLevel)
        {
            _levelMakerMainThreadActions.MainThread_CreateLevelParentAndLineObjects(prototypeLevel.NumberOfColorInLevel);
            _levelMakerMainThreadActions.MainThread_CreateBottlesAndAssignPositions(prototypeLevel);
            _levelMakerMainThreadActions.MainThread_GetLevelParent();
        }
    }
}