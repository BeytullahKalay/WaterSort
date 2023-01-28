using System.Threading;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    [RequireComponent(typeof(LevelColorController))]
    [RequireComponent(typeof(CreateBottlesForLevel))]
    [RequireComponent(typeof(LevelBottlesAligner))]
    public class LevelMaker : MonoBehaviour
    {
        [Header("Bottle Sorting Values")] [SerializeField] [Range(0, 1)]
        private float bottleDistanceX = .01f;

        [SerializeField] [Range(0, 1)] private float bottleStartPosY = .75f;
        [SerializeField] [Range(0, 1)] private float bottleDistanceY = .01f;


        [SerializeField] private GameObject bottle;

        [SerializeField] private Data _data;

        [Space(20)] [SerializeField] private GameObject lastCreatedParent;
        public bool NoMatches;
        public bool RainbowBottle;


        private int _createdBottles;
        private int _numberOfBottlesCreate;
        private int _totalWaterCount;


        // private GameObject _levelParent;
        // private GameObject _line1;
        // private GameObject _line2;
        private GameObject _obj;

        private Thread _myThread;

        private LevelColorController _levelColorController;
        private CreateBottlesForLevel _createBottlesForLevel;
        private LevelBottlesAligner _levelBottlesAligner;

        private void OnEnable()
        {
            EventManager.CreateLevel += CreateNewLevel_GUIButton;
            EventManager.AddExtraEmptyBottle += AddExtraEmptyBottle;
            EventManager.CreatePrototype += CreateLevelFromPrototype;
        }

        private void OnDisable()
        {
            EventManager.CreateLevel -= CreateNewLevel_GUIButton;
            EventManager.AddExtraEmptyBottle -= AddExtraEmptyBottle;
            EventManager.CreatePrototype -= CreateLevelFromPrototype;
        }

        private void Awake()
        {
            _levelColorController = GetComponent<LevelColorController>();
            _createBottlesForLevel = GetComponent<CreateBottlesForLevel>();
            _levelBottlesAligner = GetComponent<LevelBottlesAligner>();

            JsonManager.TryGetLevelCreateDataFromJson(_data);
            CheckNamingIndexPlayerPref();
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

            _createBottlesForLevel.CreateBottles(_numberOfBottlesCreate, NoMatches, RainbowBottle, ref _totalWaterCount,
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
            Dispatcher.Instance.Invoke(() => SetBottlePosition(numberOfBottleToCreate, tempBottle, createdBottles));
        }

        private void SetBottlePosition(int numberOfBottleToCreate, Bottle tempBottle, int createdBottles)
        {
            tempBottle.FindPositionAndAssignToPos(numberOfBottleToCreate, createdBottles, bottleDistanceX,
                bottleStartPosY, bottleDistanceY);
        }

        private void AddExtraEmptyBottle()
        {
            var gm = GameManager.Instance;

            // initialize extra bottle
            Bottle extraBottleHelper = new Bottle(-1);
            var extraBottle = InitializeBottle();
            extraBottle.HelperBottle = extraBottleHelper;
            extraBottle.NumberOfColorsInBottle = 0;

            // add new bottle to list
            var bottleControllerList = gm.bottleControllers;
            bottleControllerList.Add(extraBottle);

            // get lines
            _levelBottlesAligner.Line1 = gm.line1.gameObject;
            _levelBottlesAligner.Line2 = gm.line2.gameObject;

            // reset parent position
            _levelBottlesAligner.Line1.transform.parent.position = Vector3.zero;


            for (int i = 0; i < bottleControllerList.Count; i++)
            {
                // New parenting
                bottleControllerList[i].transform.SetParent(null);
                bottleControllerList[i].HelperBottle.ParentNum = FindParent(bottleControllerList.Count, i);

                if (bottleControllerList[i].HelperBottle.ParentNum == 0)
                    bottleControllerList[i].transform.SetParent(_levelBottlesAligner.Line1.transform);
                else if (bottleControllerList[i].HelperBottle.ParentNum == 1)
                    bottleControllerList[i].transform.SetParent(_levelBottlesAligner.Line2.transform);

                // new bottle positioning
                bottleControllerList[i].transform.position = Vector3.zero;
                bottleControllerList[i].HelperBottle.FindPositionAndAssignToPos(bottleControllerList.Count, i,
                    bottleDistanceX, bottleStartPosY, bottleDistanceY);
                bottleControllerList[i].transform.position = bottleControllerList[i].HelperBottle.GetOpenPosition();
            }

            // align bottles
            _levelBottlesAligner.AlignBottles();

            // set origin position of bottles
            for (int i = 0; i < bottleControllerList.Count; i++)
            {
                bottleControllerList[i].SetOriginalPositionTo(bottleControllerList[i].transform.position);
            }
        }

        private int FindParent(float numberOfBottleToCreate, int createdBottles)
        {
            return (createdBottles < (numberOfBottleToCreate / 2) ? 0 : 1);
        }

        private void MainThread_CreateBottlesAndAssignPositions(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() => CreateBottlesAndAssignPositions(allBottles));
        }

        private void CreateBottlesAndAssignPositions(AllBottles AllBottlesInLevel)
        {
            for (int i = 0; i < AllBottlesInLevel._allBottles.Count; i++)
            {
                var newBottle = InitializeBottle();
                newBottle.HelperBottle = AllBottlesInLevel._allBottles[i];
                newBottle.NumberOfColorsInBottle = AllBottlesInLevel._allBottles[i].NumberOfColorsInBottle;
                newBottle.transform.position = AllBottlesInLevel._allBottles[i].GetOpenPosition();
                AllBottlesInLevel._allBottles[i].BottleColors.CopyTo(newBottle.BottleColors, 0);
                Parenting(newBottle);
            }

            _levelBottlesAligner.AlignBottles();
        }

        private void Parenting(BottleController newBottle)
        {
            if (newBottle.HelperBottle.ParentNum == 0)
            {
                newBottle.transform.SetParent(_levelBottlesAligner.Line1.transform);
            }
            else if (newBottle.HelperBottle.ParentNum == 1)
            {
                newBottle.transform.SetParent(_levelBottlesAligner.Line2.transform);
            }
        }

        private BottleController InitializeBottle()
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

            var objBottleControllerScript = _obj.GetComponent<BottleController>();

            objBottleControllerScript.BottleSorted = false;

            return objBottleControllerScript;
        }

        public void SetNumberOfColorToCreate(int value)
        {
            _levelColorController.NumberOfColorsToCreate = value;
        }
    }
}