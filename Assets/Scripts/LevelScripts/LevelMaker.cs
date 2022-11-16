using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class LevelMaker : MonoBehaviour
{
    [Header("Bottle Sorting Values")] [SerializeField] [Range(0, 1)]
    private float bottleDistanceX = .01f;

    [SerializeField] [Range(0, 1)] private float bottleStartPosY = .75f;
    [SerializeField] [Range(0, 1)] private float bottleDistanceY = .01f;


    [SerializeField] private GameObject bottle;

    [Space(10)] [SerializeField] private LineRenderer lineRenderer;

    [Header("Databases")] [SerializeField] private Colors _colorsdb;
    [SerializeField] private Data _data;

    [Header("Level Maker")] [SerializeField]
    private int numberOfColorsToCreate = 2;

    [SerializeField] private List<Color> selectedColors = new List<Color>();
    private List<MyColors> _myColorsList = new List<MyColors>();

    [Space(20)] [SerializeField] private GameObject lastCreatedParent;
    [SerializeField] private bool noMatches;


    private int _createdBottles;
    private int _numberOfBottlesCreate;
    private int _totalWaterCount;


    private GameObject _levelParent;
    private GameObject _line1;
    private GameObject _line2;
    private GameObject _obj;

    private Thread _myThread;

    private void Update()
    {
        Dispatcher.Instance.InvokePending();

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("CALLED!!a");
            CreateNewLevel_GUIButton();
        }
    }

    // using by inspector gui
    public void CreateNewLevel_GUIButton()
    {
        if (_myThread == null || !_myThread.IsAlive)
            _myThread = new Thread(CreateLevelActions);

        _myThread.Start();
    }

    private void CreateLevelActions()
    {
        CreateLevel();

        #region commnets

        // // check in do while
        // do
        // {
        //     Debug.Log("in while");
        //
        //     createdBottlesContainer.Clear();
        //
        //     CreateLevel();
        //     ColorNumerator.NumerateColors(selectedColors);
        // } while (!new AllBottles(createdBottlesContainer).IsSolvable());
        //
        // Debug.Log("Solvable");
        //
        // //  Create bottles here
        //
        //
        // //lastCreatedParent.SetActive(false);
        // Async_SetActive(lastCreatedParent, false);
        //
        // //SaveLevelAsPrefab();
        // Async_SaveLevelAsPrefab();
        //
        // //if (_myThread.IsAlive)
        //
        // _myThread.Abort();
        //
        // Debug.Log("Solvable");

        #endregion
    }

    private void Async_SetActive(GameObject obj, bool state)
    {
        //Thread.Sleep(50);
        Dispatcher.Instance.Invoke(() => obj.SetActive(state));
    }

    private void CreateLevel()
    {
        _data.CreatedBottles.Clear();
        _createdBottles = 0;
        selectedColors.Clear();

        SelectColorsToCreate();

        CreateColorObjects();

        _totalWaterCount = selectedColors.Count * 4;

        RandomizeNumberOfBottle();
        
        CreateBottles(_numberOfBottlesCreate, noMatches);

        AllBottles allBottles = new AllBottles(_data.CreatedBottles);
        ColorNumerator.NumerateColors(selectedColors);

        if (allBottles.IsSolvable())
        {
            Debug.Log("Solvable");
            
            //CreateLevelParentAndLineObjects();
            MainThread_CreateLevelParentAndLineObjects();
        
            //CreateBottlesAndAssignPositions();
            MainThread_CreateBottlesAndAssignPositions();
            
            MainThread_SaveLevelAsPrefab();
        }
        else
        {
            CreateLevel();
        }
        
    }

    // THIS WILL CHANGE
    private void RandomizeNumberOfBottle()
    {
        var hasString = "ExtraBottle " + _data.GetAmountOfExtraBottleIndex().ToString();
        var rand = new Unity.Mathematics.Random((uint)hasString.GetHashCode());
        _numberOfBottlesCreate = rand.NextInt(numberOfColorsToCreate + 1, numberOfColorsToCreate + 3);
    }

    private void SelectColorsToCreate()
    {
        while (selectedColors.Count < numberOfColorsToCreate)
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

    private void MainThread_CreateLevelParentAndLineObjects()
    {
        Thread.Sleep(50);
        Dispatcher.Instance.Invoke(() => CreateLevelParentAndLineObjects());
    }

    private void CreateLevelParentAndLineObjects()
    {
        _levelParent = new GameObject("LevelParent");
        _line1 = new GameObject("Line1");
        _line2 = new GameObject("Line2");
        _line2.transform.parent = _line1.transform.parent = _levelParent.transform;

        _levelParent.AddComponent<LevelParent>();
        _levelParent.GetComponent<LevelParent>().numberOfColor = numberOfColorsToCreate;

        lastCreatedParent = _levelParent;
    }


    private void CreateBottles(float numberOfBottleToCreate, bool matchState)
    {
        for (int i = 0; i < numberOfBottleToCreate; i++)
        {
            Bottle tempBottle = new Bottle(i);
            DecreaseTotalWaterCount(tempBottle);
            GetRandomColorForBottle(tempBottle, matchState);
            

            MainThread_SetBottlePosition(numberOfBottleToCreate, tempBottle,_createdBottles);

            tempBottle.ParentNum = FindParent(numberOfBottleToCreate);

            _data.CreatedBottles.Add(tempBottle);
            
            // increase created bottle amount
            _createdBottles++;
        }
    }

    private void MainThread_SetBottlePosition(float numberOfBottleToCreate, Bottle tempBottle,int createdBottles)
    {
        Dispatcher.Instance.Invoke(() => SetBottlePosition(numberOfBottleToCreate, tempBottle,createdBottles));
    }

    private void SetBottlePosition(float numberOfBottleToCreate, Bottle tempBottle,int createdBottles)
    {
        //tempBottle.SetOpenPositionTo(FindPosition(numberOfBottleToCreate));
        tempBottle.FindPositionAndAssignToPos(numberOfBottleToCreate, createdBottles, bottleDistanceX, bottleStartPosY,
            bottleDistanceY);
    }


    private int FindParent(float numberOfBottleToCreate)
    {
        return (_createdBottles < (numberOfBottleToCreate / 2) ? 0 : 1);
    }


    private void GetRandomColorForBottle(Bottle tempBottle, bool matchState)
    {
        for (int j = 0; j < tempBottle.BottleColorsHashCodes.Length; j++)
        {
            var color = GetColorFromList(matchState, tempBottle, j - 1);
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

    private void MainThread_CreateBottlesAndAssignPositions()
    {
        Dispatcher.Instance.Invoke(() => CreateBottlesAndAssignPositions());
    }

    private void CreateBottlesAndAssignPositions()
    {
        for (int i = 0; i < _data.CreatedBottles.Count; i++)
        {
            var newBottle = InitializeBottle();
            newBottle.NumberOfColorsInBottle = _data.CreatedBottles[i].NumberOfColorsInBottle;
            newBottle.transform.position = _data.CreatedBottles[i].GetOpenPosition();
            _data.CreatedBottles[i].BottleColors.CopyTo(newBottle.BottleColors, 0);


            if (_data.CreatedBottles[i].ParentNum == 0)
            {
                newBottle.transform.SetParent(_line1.transform);
            }
            else if (_data.CreatedBottles[i].ParentNum == 1)
            {
                newBottle.transform.SetParent(_line2.transform);
            }
        }

        AlignBottles();
    }

    private BottleController InitializeBottle()
    {
        _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

        var objBottleControllerScript = _obj.GetComponent<BottleController>();

        objBottleControllerScript.BottleSorted = false;

        objBottleControllerScript.LineRenderer = lineRenderer;
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


    private Color GetColorFromList(bool matchState, Bottle tempBottle, int checkIndex)
    {
        if (_myColorsList.Count > 0)
        {
            var hashString = "GetRandomColor " + _data.GetColorPickerRandomIndex().ToString();
            var rand = new Unity.Mathematics.Random((uint)hashString.GetHashCode());

            int randomColorIndex = rand.NextInt(0, _myColorsList.Count);
            var color = _myColorsList[randomColorIndex].Color;


            if (checkIndex >= 0)
            {
                while (matchState && color.GetHashCode() == tempBottle.GetColorHashCodeAtPosition(checkIndex))
                {
                    if (_myColorsList.Count < 2) break;
                    
                    var newHashString = "GetRandomColor " + _data.GetColorPickerRandomIndex().ToString();
                    var newRandomSeed = new Unity.Mathematics.Random((uint)newHashString.GetHashCode());

                    randomColorIndex = newRandomSeed.NextInt(0, _myColorsList.Count);
                    color = _myColorsList[randomColorIndex].Color;
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

    // using by inspector gui
    public void SaveLevelAsPrefab()
    {
        string levelPrefabPath = "Assets/Prefabs/Levels/" + "Level" + ".prefab";
        levelPrefabPath = AssetDatabase.GenerateUniqueAssetPath(levelPrefabPath);
        var obj = PrefabUtility.SaveAsPrefabAssetAndConnect(_levelParent, levelPrefabPath, InteractionMode.UserAction);


        var level = ScriptableObject.CreateInstance<Level>();
        level.LevelPrefab = obj.GetComponent<LevelParent>();
        string levelScriptableObjectPath = "Assets/SCOB/Level/" + "Level_" + ".asset";
        levelScriptableObjectPath = AssetDatabase.GenerateUniqueAssetPath(levelScriptableObjectPath);
        AssetDatabase.CreateAsset(level, levelScriptableObjectPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Destroy(_levelParent);
    }

    private void MainThread_SaveLevelAsPrefab()
    {
        Dispatcher.Instance.Invoke(() => SaveLevelAsPrefab());
    }
}