using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class LevelMaker : MonoBehaviour
{
    [Header("Bottle Sorting Values")] [SerializeField] [Range(0, 1)]
    private float bottleDistanceX = .01f;

    [SerializeField] [Range(0, 1)] private float bottleStartPosY = .75f;
    [SerializeField] [Range(0, 1)] private float bottleDistanceY = .01f;


    [SerializeField] private GameObject bottle;

    [Header("Databases")] [SerializeField] private Colors _colorsdb;
    [SerializeField] private Data _data;

    [Header("Level Maker")] public int NumberOfColorsToCreate = 2;

    [SerializeField] private List<Color> selectedColors = new List<Color>();
    private List<MyColors> _myColorsList = new List<MyColors>();

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
        TryGetLevelCreateDataFromJson();
        CheckNamingIndexPlayerPref();
    }

    private void TryGetLevelCreateDataFromJson()
    {
        string path = Paths.LevelCreationDataPath;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _data = JsonUtility.FromJson<Data>(json);
        }
        else
        {
            Debug.Log("Data not exists. Creating new one");
            string json = JsonUtility.ToJson(_data);
            File.WriteAllText(path, json);
        }
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
        
        if (_myThread is not { IsAlive: true })
        {
            _myThread = new Thread(CreateLevelPrototype);
            
            _myThread.Start();
        }
        else
        {
            Debug.LogWarning("User try starting started thread!");
        }
    }

    private void CreateLevelPrototype()
    {
        _data.CreatedBottles.Clear();
        _createdBottles = 0;
        selectedColors.Clear();

        SelectColorsToCreate();

        CreateColorObjects();

        _totalWaterCount = selectedColors.Count * 4;

        RandomizeNumberOfBottle();

        CreateBottles(_numberOfBottlesCreate, NoMatches, RainbowBottle);

        AllBottles allBottles = new AllBottles(_data.CreatedBottles);
        ColorNumerator.NumerateColors(selectedColors);
        
        if (allBottles.IsSolvable())
        {
            Debug.Log("Solvable");

            allBottles.numberOfColorInLevel = NumberOfColorsToCreate;

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
        Dispatcher.Instance.Invoke(SaveLevelCreateDataToJson);
    }

    private void SaveLevelCreateDataToJson()
    {
        string json = JsonUtility.ToJson(_data);
        string path = Paths.LevelCreationDataPath;
        File.WriteAllText(path, json);
    }

    private void CreateLevelFromPrototype(AllBottles prototypeLevel)
    {
        MainThread_CreateLevelParentAndLineObjects(prototypeLevel.numberOfColorInLevel);
        MainThread_CreateBottlesAndAssignPositions(prototypeLevel);
        MainThread_GetLevelParent();
    }

    private void MainThread_GetLevelParent()
    {
        Dispatcher.Instance.Invoke(() => { EventManager.GetLevelParent?.Invoke(lastCreatedParent); });
    }

    private void MainThread_SaveToJson(AllBottles allBottles)
    {
        Dispatcher.Instance.Invoke(() => SaveToJson(allBottles));
    }

    private void SaveToJson(AllBottles allBottles)
    {
        string json = JsonUtility.ToJson(allBottles);
        string path = Path.Combine(Application.persistentDataPath,
            PlayerPrefs.GetInt(PlayerPrefNames.NamingIndex) % 4 + "data.json");
        File.WriteAllText(path, json);
        EventManager.SaveJsonFilePath?.Invoke(path);
        PlayerPrefs.SetInt(PlayerPrefNames.NamingIndex, PlayerPrefs.GetInt(PlayerPrefNames.NamingIndex) + 1);
    }

    private void RandomizeNumberOfBottle()
    {

        var hasString = "ExtraBottle " + _data.GetAmountOfExtraBottleIndex().ToString();
        var rand = new Unity.Mathematics.Random((uint)hasString.GetHashCode());
        _numberOfBottlesCreate = rand.NextInt(NumberOfColorsToCreate + 1, NumberOfColorsToCreate + 3);
    }

    private void SelectColorsToCreate()
    {
        while (selectedColors.Count < NumberOfColorsToCreate)
        {
            var selectedColor = _colorsdb.GetRandomColor(_data.GetBottleColorRandomIndex());

            if (!selectedColors.Contains(selectedColor))
                selectedColors.Add(selectedColor);
        }
    }

    private void CreateColorObjects()
    {
        foreach (var color in selectedColors)
        {
            MyColors colorObj = new MyColors(color);
            _myColorsList.Add(colorObj);
        }
    }

    private void MainThread_CreateLevelParentAndLineObjects(int numberOfColorInLevel)
    {
        Thread.Sleep(50);
        Dispatcher.Instance.Invoke(() => CreateLevelParentAndLineObjects(numberOfColorInLevel));
    }

    private void CreateLevelParentAndLineObjects(int numberOfColorInlevel)
    {
        _levelParent = new GameObject("LevelParent");
        _line1 = new GameObject("Line1");
        _line2 = new GameObject("Line2");
        _line2.transform.parent = _line1.transform.parent = _levelParent.transform;

        _levelParent.AddComponent<LevelParent>();
        _levelParent.GetComponent<LevelParent>().NumberOfColor = numberOfColorInlevel;

        _levelParent.GetComponent<LevelParent>().GetLines(_line1.transform, _line2.transform);
        lastCreatedParent = _levelParent;
    }


    private void CreateBottles(int numberOfBottleToCreate, bool matchState, bool rainbowBottle)
    {
        for (int i = 0; i < numberOfBottleToCreate; i++)
        {
            Bottle tempBottle = new Bottle(i);
            DecreaseTotalWaterCount(tempBottle);
            GetRandomColorForBottle(tempBottle, matchState, rainbowBottle);

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
        tempBottle.FindPositionAndAssignToPos(numberOfBottleToCreate, createdBottles, bottleDistanceX, bottleStartPosY,
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

    private void GetRandomColorForBottle(Bottle tempBottle, bool matchState, bool rainbowBottle)
    {
        for (int j = 0; j < tempBottle.BottleColorsHashCodes.Length; j++)
        {
            var color = GetColorFromList(matchState, rainbowBottle, tempBottle, j - 1);
            tempBottle.BottleColorsHashCodes[j] = color.GetHashCode();
            tempBottle.BottleColors[j] = color;
        }

        InitializeBottleNumberedStack(tempBottle);
    }

    private void InitializeBottleNumberedStack(Bottle comingBottle)
    {
        foreach (var colorHashCode in comingBottle.BottleColorsHashCodes)
        {
            int emptyColorHashCode = 532676608;
            if (colorHashCode != emptyColorHashCode)
                comingBottle.NumberedBottleStack.Push(colorHashCode);
        }

        comingBottle.CheckIsSorted();
        comingBottle.CalculateTopColorAmount();
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

    private Color GetColorFromList(bool matchState, bool rainbowBottle, Bottle tempBottle, int checkIndex)
    {
        if (_myColorsList.Count > 0)
        {
            var randomColorIndex = GetRandomColorIndex();
            var color = _myColorsList[randomColorIndex].Color;
            

            if (checkIndex >= 0)
            {
                if (rainbowBottle)
                {
                    var colorMatched = false;

                    var iteration = 0;
                    
                    do
                    {
                        if (iteration > 200) break;

                        iteration++;
                        
                        colorMatched = false;

                        if (_myColorsList.Count < 2) break;
                        
                        for (int i = 0; i <= checkIndex; i++)
                        {
                            if (color.GetHashCode() == tempBottle.GetColorHashCodeAtPosition(i))
                            {   

                                randomColorIndex = GetRandomColorIndex();
                                color = _myColorsList[randomColorIndex].Color;

                                colorMatched = true;
                                break;
                            }
                        }
                    } while (colorMatched);
                }
                else
                {
                    while (matchState && color.GetHashCode() == tempBottle.GetColorHashCodeAtPosition(checkIndex))
                    {
                        if (_myColorsList.Count < 2) break;

                        randomColorIndex = GetRandomColorIndex();
                        color = _myColorsList[randomColorIndex].Color;
                    }
                }
            }

            _myColorsList[randomColorIndex].Amount++;

            if (_myColorsList[randomColorIndex].MoreThan4())
            {
                _myColorsList.RemoveAt(randomColorIndex);
            }


            return color;
        }
        else
        {
            return Color.black;
        }
    }

    private int GetRandomColorIndex()
    {
        var hashString = "GetRandomColor " + _data.GetColorPickerRandomIndex().ToString();
        var rand = new Unity.Mathematics.Random((uint)hashString.GetHashCode());
        int randomColorIndex = rand.NextInt(0, _myColorsList.Count);
        return randomColorIndex;
    }
}