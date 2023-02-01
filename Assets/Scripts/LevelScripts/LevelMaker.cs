using System.Threading;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    [RequireComponent(typeof(LevelColorController))]
    [RequireComponent(typeof(CreateBottlesForLevel))]
    [RequireComponent(typeof(LevelBottlesAligner))]
    [RequireComponent(typeof(LevelMakerStateController))]
    [RequireComponent(typeof(LevelMakerBottlePositioning))]
    [RequireComponent(typeof(LevelMakerCreateBottle))]
    
    public class LevelMaker : MonoBehaviour
    {
        [SerializeField] private GameObject bottle;

        [SerializeField] private Data _data;


        private int _createdBottles;
        private int _numberOfBottlesCreate;
        private int _totalWaterCount;

        private Thread _myThread;

        private LevelColorController _levelColorController;
        private CreateBottlesForLevel _createBottlesForLevel;
        private LevelBottlesAligner _levelBottlesAligner;
        private LevelMakerStateController _levelMakerStateController;
        private LevelMakerBottlePositioning _levelMakerBottlePositioning;
        private LevelMakerCreateBottle _levelMakerCreateBottle;

        public LevelMakerStateController LevelMakerStateController { get; private set; }
        public LevelColorController LevelColorController { get; private set; }

        
        private void Awake()
        {
            _levelColorController = GetComponent<LevelColorController>();
            _createBottlesForLevel = GetComponent<CreateBottlesForLevel>();
            _levelBottlesAligner = GetComponent<LevelBottlesAligner>();
            _levelMakerStateController = GetComponent<LevelMakerStateController>();
            _levelMakerBottlePositioning = GetComponent<LevelMakerBottlePositioning>();
            _levelMakerCreateBottle = GetComponent<LevelMakerCreateBottle>();

            LevelMakerStateController = _levelMakerStateController;
            LevelColorController = _levelColorController;

            JsonManager.TryGetLevelCreateDataFromJson(_data);
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

        private void Update()
        {
            Dispatcher.Instance.InvokePending();
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
            _data.CreatedBottles.Clear();
            _createdBottles = 0;
            _levelColorController.SelectedColors.Clear();

            _levelColorController.SelectColorsToCreate(_data);

            _levelColorController.CreateColorObjects();

            _totalWaterCount = _levelColorController.SelectedColors.Count * 4;
            
            _numberOfBottlesCreate = LevelMakerHelper.RandomizeNumberOfBottle(_data, _levelColorController);

            _createBottlesForLevel.CreateBottles(_numberOfBottlesCreate,  _levelMakerStateController.NoMatches,_levelMakerStateController. RainbowBottle, ref _totalWaterCount,
                _levelColorController, _data, _createdBottles, MainThread_SetBottlePosition);

            AllBottles allBottles = new AllBottles(_data.CreatedBottles);
            ColorNumerator.NumerateColors(_levelColorController.SelectedColors);

            if (allBottles.IsSolvable())
            {
                Debug.Log("Solvable");

                allBottles.NumberOfColorInLevel = _levelColorController.NumberOfColorsToCreate;

                MainThread_SaveToJson(allBottles);

                MainThread_SaveLevelCreateDataToJson();
            }
            else
            {
                CreateLevelPrototype();
            }
        }

        private void MainThread_SaveLevelCreateDataToJson()
        {
            Dispatcher.Instance.Invoke(CallSaveLevelDataToJson);
        }

        private void CallSaveLevelDataToJson()
        {
            JsonManager.SaveLevelCreateDataToJson(ref _data);
        }

        private void CreateLevelFromPrototype(AllBottles prototypeLevel)
        {
            MainThread_CreateLevelParentAndLineObjects(prototypeLevel.NumberOfColorInLevel);
            MainThread_CreateBottlesAndAssignPositions(prototypeLevel);
            MainThread_GetLevelParent();
        }

        private void MainThread_GetLevelParent()
        {
            Dispatcher.Instance.Invoke(() => { EventManager.GetLevelParent?.Invoke(_levelBottlesAligner.LastCreatedParent); });
        }

        private void MainThread_SaveToJson(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() => JsonManager.SaveToJson(allBottles));
        }

        private void MainThread_CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            Thread.Sleep(50);
            Dispatcher.Instance.Invoke(() => _levelBottlesAligner.CreateLevelParentAndLineObjects(numberOfColorInLevel));
        }

        private void MainThread_SetBottlePosition(int numberOfBottleToCreate, Bottle tempBottle, int createdBottles)
        {
            Dispatcher.Instance.Invoke(() => _levelMakerBottlePositioning.SetBottlePosition(numberOfBottleToCreate, tempBottle, createdBottles));
        }

        private void MainThread_CreateBottlesAndAssignPositions(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() =>_levelMakerCreateBottle.CreateBottlesAndAssignPositions(allBottles,_levelBottlesAligner));
        }
    }
}