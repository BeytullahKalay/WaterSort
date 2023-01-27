using System.Threading;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    [RequireComponent(typeof(LevelColorController))]
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


        private GameObject _levelParent;
        private GameObject _line1;
        private GameObject _line2;
        private GameObject _obj;

        private Thread _myThread;

        private LevelColorController _levelColorController;

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

            RandomizeNumberOfBottle();

            CreateBottles(_numberOfBottlesCreate, NoMatches, RainbowBottle);

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
            Dispatcher.Instance.Invoke(() => { EventManager.GetLevelParent?.Invoke(lastCreatedParent); });
        }

        private void MainThread_SaveToJson(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() => JsonManager.SaveToJson(allBottles));
        }

        private void RandomizeNumberOfBottle()
        {
            var hasString = "ExtraBottle " + _data.GetAmountOfExtraBottleIndex().ToString();
            var rand = new Unity.Mathematics.Random((uint)hasString.GetHashCode());
            _numberOfBottlesCreate = rand.NextInt(_levelColorController.NumberOfColorsToCreate + 1,
                _levelColorController.NumberOfColorsToCreate + 3);
        }

        private void MainThread_CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            Thread.Sleep(50);
            Dispatcher.Instance.Invoke(() => CreateLevelParentAndLineObjects(numberOfColorInLevel));
        }

        private void CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            _levelParent = new GameObject("LevelParent");
            _line1 = new GameObject("Line1");
            _line2 = new GameObject("Line2");
            _line2.transform.parent = _line1.transform.parent = _levelParent.transform;

            _levelParent.AddComponent<LevelParent>();
            _levelParent.GetComponent<LevelParent>().NumberOfColor = numberOfColorInLevel;

            _levelParent.GetComponent<LevelParent>().GetLines(_line1.transform, _line2.transform);
            lastCreatedParent = _levelParent;
        }


        private void CreateBottles(int numberOfBottleToCreate, bool matchState, bool rainbowBottle)
        {
            for (int i = 0; i < numberOfBottleToCreate; i++)
            {
                Bottle tempBottle = new Bottle(i);
                DecreaseTotalWaterCount(tempBottle);
                _levelColorController.GetRandomColorForBottle(tempBottle, matchState, rainbowBottle, _data);

                MainThread_SetBottlePosition(numberOfBottleToCreate, tempBottle, _createdBottles);

                tempBottle.ParentNum = FindParent(numberOfBottleToCreate, _createdBottles);

                _data.CreatedBottles.Add(tempBottle);

                // increase created bottle amount
                _createdBottles++;
            }
        }

        private void MainThread_SetBottlePosition(int numberOfBottleToCreate, Bottle tempBottle, int createdBottles)
        {
            Dispatcher.Instance.Invoke(() => SetBottlePosition(numberOfBottleToCreate, tempBottle, createdBottles));
        }

        private void SetBottlePosition(int numberOfBottleToCreate, Bottle tempBottle, int createdBottles)
        {
            tempBottle.FindPositionAndAssignToPos(numberOfBottleToCreate, createdBottles, bottleDistanceX,
                bottleStartPosY,
                bottleDistanceY);
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
            _line1 = gm.line1.gameObject;
            _line2 = gm.line2.gameObject;

            // reset parent position
            _line1.transform.parent.position = Vector3.zero;


            for (int i = 0; i < bottleControllerList.Count; i++)
            {
                // New parenting
                bottleControllerList[i].transform.SetParent(null);
                bottleControllerList[i].HelperBottle.ParentNum = FindParent(bottleControllerList.Count, i);

                if (bottleControllerList[i].HelperBottle.ParentNum == 0)
                    bottleControllerList[i].transform.SetParent(_line1.transform);
                else if (bottleControllerList[i].HelperBottle.ParentNum == 1)
                    bottleControllerList[i].transform.SetParent(_line2.transform);

                // new bottle positioning
                bottleControllerList[i].transform.position = Vector3.zero;
                bottleControllerList[i].HelperBottle.FindPositionAndAssignToPos(bottleControllerList.Count, i,
                    bottleDistanceX, bottleStartPosY, bottleDistanceY);
                bottleControllerList[i].transform.position = bottleControllerList[i].HelperBottle.GetOpenPosition();
            }

            // align bottles
            AlignBottles();

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

        private void DecreaseTotalWaterCount(Bottle tempBottle)
        {
            if (_totalWaterCount >= 4)
            {
                tempBottle.NumberOfColorsInBottle = 4;
                _totalWaterCount -= 4;
            }
            else
            {
                tempBottle.NumberOfColorsInBottle = 0;
            }
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

            AlignBottles();
        }

        private void Parenting(BottleController newBottle)
        {
            if (newBottle.HelperBottle.ParentNum == 0)
            {
                newBottle.transform.SetParent(_line1.transform);
            }
            else if (newBottle.HelperBottle.ParentNum == 1)
            {
                newBottle.transform.SetParent(_line2.transform);
            }
        }

        private BottleController InitializeBottle()
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

            var objBottleControllerScript = _obj.GetComponent<BottleController>();

            objBottleControllerScript.BottleSorted = false;

            return objBottleControllerScript;
        }

        private void AlignBottles()
        {
            var line1Right = _line1.transform.GetChild(_line1.transform.childCount - 1);
            var rightOfScreen = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            var distanceToRight = Mathf.Abs(rightOfScreen - line1Right.transform.position.x);


            var x = Mathf.Abs(distanceToRight / 2);
            var newParentPos = _line1.transform.parent.position;
            newParentPos.x = x;
            _line1.transform.parent.position = newParentPos;
        }

        public void SetNumberOfColorToCreate(int value)
        {
            _levelColorController.NumberOfColorsToCreate = value;
        }
    }
}