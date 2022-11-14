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

        //CreateLevelParentAndLineObjects();
        //Async_CreateLevelParentAndLineObjects();

        //CreateBottles(_numberOfBottlesCreate, noMatches);
        Async_CreateBottles(_numberOfBottlesCreate, noMatches);
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

    private void Async_CreateLevelParentAndLineObjects()
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

    private void Async_CreateBottles(float numberOfBottleToCreate, bool matchState)
    {
        //Thread.Sleep(50);
        Dispatcher.Instance.Invoke(() => CreateBottles(numberOfBottleToCreate, matchState));
    }


    private void CreateBottles(float numberOfBottleToCreate, bool matchState)
    {
        for (int i = 0; i < numberOfBottleToCreate; i++)
        {
            Bottle tempBottle = new Bottle(-1);
            DecreaseTotalWaterCount(tempBottle);
            GetRandomColorForBottle(tempBottle, matchState);

            tempBottle.SetOpenPositionTo(FindPosition(numberOfBottleToCreate));
            tempBottle.ParentNum = FindParent(numberOfBottleToCreate);
            //tempBottle.ParentTransform = FindParent(numberOfColorsToCreate, _createdBottles);
            //PositioningAndParenting(numberOfBottleToCreate, pos);

            tempBottle.CheckIsSorted();
            _data.CreatedBottles.Add(tempBottle);


            // increase created bottle amount
            _createdBottles++;
        }

        Debug.Log(_data.CreatedBottles.Count);

        AllBottles allBottles = new AllBottles(_data.CreatedBottles);
        ColorNumerator.NumerateColors(selectedColors);

        if (allBottles.IsSolvable())
        {
            Debug.Log("Solvable");
            CreateLevelParentAndLineObjects();
            CreateBottlesAndAssignPositions();
            AlignBottles();
            //Async_CreateLevelParentAndLineObjects();
            //AlignBottles();
        }
        else
        {
            Debug.Log("Not Solvable");
        }
    }

    private int FindParent(float numberOfBottleToCreate)
    {
        return (_createdBottles < (numberOfBottleToCreate / 2) ? 0 : 1);
    }

    private Vector3 FindPosition(float numberOfBottleToCreate)
    {
        var posA = new Vector3(_createdBottles % (numberOfBottleToCreate / 2) * bottleDistanceX,
            bottleStartPosY - bottleDistanceY * Mathf.Floor(_createdBottles / (numberOfBottleToCreate / 2)), 0);

        Vector3 pos = Camera.main.ViewportToWorldPoint(posA);
        pos.z = 0;
        return pos;
    }

    private void GetRandomColorForBottle(Bottle tempBottle, bool matchState)
    {
        for (int j = 0; j < tempBottle.BottleColorsHashCodes.Length; j++)
        {
            var color = GetColorFromList(matchState, tempBottle, j - 1);
            tempBottle.BottleColorsHashCodes[j] = color.GetHashCode();
            tempBottle.BottleColors[j] = color;
        }
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

    private void CreateBottlesAndAssignPositions()
    {
        for (int i = 0; i < _data.CreatedBottles.Count; i++)
        {
            var newBottle = InitializeBottle();
            newBottle.NumberOfColorsInBottle = _data.CreatedBottles[i].NumberOfColorsInBottle;
            newBottle.transform.position = _data.CreatedBottles[i].GetOpenPosition();
            _data.CreatedBottles[i].BottleColors .CopyTo(newBottle.BottleColors,0);
            

            if (_data.CreatedBottles[i].ParentNum == 0)
            {
                newBottle.transform.SetParent(_line1.transform);
            }
            else if (_data.CreatedBottles[i].ParentNum == 1)
            {
                newBottle.transform.SetParent(_line2.transform);
            }
        }
    }

    private BottleController InitializeBottle()
    {
        _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

        var objBottleControllerScript = _obj.GetComponent<BottleController>();

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
            Random.InitState(_data.GetColorPickerRandomIndex());
            int randomColorIndex = Random.Range(0, _myColorsList.Count);
            var color = _myColorsList[randomColorIndex].Color;

            if (checkIndex >= 0)
            {
                while (matchState && color.GetHashCode() == tempBottle.GetColorHashCodeAtPosition(checkIndex))
                {
                    if (_myColorsList.Count < 2) break;

                    randomColorIndex = Random.Range(0, _myColorsList.Count);
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

    private void Async_SaveLevelAsPrefab()
    {
        Dispatcher.Instance.Invoke(() => SaveLevelAsPrefab());
    }
}