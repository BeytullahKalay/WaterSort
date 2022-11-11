using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    [Header("Color Database")] [SerializeField]
    private Colors _colorsdb;

    [Header("Level Maker")] [SerializeField]
    private int numberOfColorsToCreate = 2;

    [SerializeField] private List<Color> selectedColors = new List<Color>();
    private List<MyColors> _myColorsList = new List<MyColors>();

    [Space(20)] [SerializeField] private GameObject lastCreatedParent;
    [SerializeField] private bool noMatches;

    [Header("Created Bottles")] public List<BottleController> createdBottlesContainer;


    private int _createdBottles;
    private int _numberOfBottlesCreate;
    private int _totalWaterCount;
    private int _randomChanger;


    private GameObject _levelParent;
    private GameObject _line1;
    private GameObject _line2;
    private GameObject _obj;

    private Thread _myThread;
    private Dispatcher _dispatcher;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
    }

    private void Update()
    {
        _dispatcher.InvokePending();

        // if (_myThread == null) return;
        //
        // if (_myThread.IsAlive)
        // {
        //     Debug.Log("_myThread Alive");
        // }
        // else
        // {
        //     Debug.Log("_myThread Dead");
        // }
    }

    // using by inspector gui
    public void CreateNewLevel_GUIButton()
    {
        if (_myThread == null || !_myThread.IsAlive)
            _myThread = new Thread(CreateLevelActions);

        _myThread.Start();
        //CreateLevelActions();
    }

    private void CreateLevelActions()
    {
        do
        {
            createdBottlesContainer.Clear();
            //DestroyImmediate(lastCreatedParent);
            CreateLevel();
            ColorNumerator.NumerateColors(selectedColors);
        } while (!new AllBottles(createdBottlesContainer).IsSolvable());

        //lastCreatedParent.SetActive(false);
        Async_SetActive(lastCreatedParent, false);

        //SaveLevelAsPrefab();
        Async_SaveLevelAsPrefab();

        //if (_myThread.IsAlive)

        _myThread.Abort();

        Debug.Log("Solvable");
    }

    private void Async_SetActive(GameObject obj, bool state)
    {
        //Thread.Sleep(50);
        _dispatcher.Invoke(() => obj.SetActive(state));
    }

    private void CreateLevel()
    {
        _createdBottles = 0;
        selectedColors.Clear();

        SelectColorsToCreate();

        CreateColorObjects();

        _totalWaterCount = selectedColors.Count * 4;

        RandomizeNumberOfBottle();

        //CreateLevelParentAndLineObjects();
        Async_CreateLevelParentAndLineObjects();

        //CreateBottles(_numberOfBottlesCreate, noMatches);
        Async_CreateBottles(_numberOfBottlesCreate, noMatches);
    }

    private void RandomizeNumberOfBottle()
    {
        var hasString = "Level " + _randomChanger.ToString();
        var rand = new Unity.Mathematics.Random((uint)hasString.GetHashCode());
        _randomChanger += 7;
        _numberOfBottlesCreate = rand.NextInt(numberOfColorsToCreate + 1, numberOfColorsToCreate + 3);

        //_numberOfBottlesCreate = Random.Range(numberOfColorsToCreate + 1, numberOfColorsToCreate + 3);
    }

    private void SelectColorsToCreate()
    {
        while (selectedColors.Count < numberOfColorsToCreate)
        {
            var selectedColor = _colorsdb.GetRandomColor();

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
        _dispatcher.Invoke(() => CreateLevelParentAndLineObjects());
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
        Thread.Sleep(50);
        _dispatcher.Invoke(() => CreateBottles(numberOfBottleToCreate, matchState));
    }


    private void CreateBottles(float numberOfBottleToCreate, bool matchState)
    {
        for (int i = 0; i < numberOfBottleToCreate; i++)
        {
            var objBottleControllerScript = InitializeBottle();

            DecreaseTotalWaterCount(objBottleControllerScript);

            GetRandomColorForBottle(objBottleControllerScript, matchState);

            var pos = FindPosition(numberOfBottleToCreate);

            PositioningAndParenting(numberOfBottleToCreate, pos);

            // add create bottle to bottle container
            createdBottlesContainer.Add(_obj.GetComponent<BottleController>());

            // increase created bottle amount
            _createdBottles++;
        }

        AlignBottles();
    }

    private void PositioningAndParenting(float numberOfBottleToCreate, Vector3 pos)
    {
        _obj.transform.position = pos;
        _obj.transform.SetParent(_createdBottles < (numberOfBottleToCreate / 2) ? _line1.transform : _line2.transform);
    }

    private Vector3 FindPosition(float numberOfBottleToCreate)
    {
        var posA = new Vector3(_createdBottles % (numberOfBottleToCreate / 2) * bottleDistanceX,
            bottleStartPosY - bottleDistanceY * Mathf.Floor(_createdBottles / (numberOfBottleToCreate / 2)), 0);

        Vector3 pos = Camera.main.ViewportToWorldPoint(posA);
        pos.z = 0;
        return pos;
    }

    private void GetRandomColorForBottle(BottleController objBottleControllerScript, bool matchState)
    {
        for (int j = 0; j < objBottleControllerScript.BottleColors.Length; j++)
        {
            objBottleControllerScript.BottleColors[j] = GetColorFromList(matchState, objBottleControllerScript, j - 1);
        }
    }

    private void DecreaseTotalWaterCount(BottleController objBottleControllerScript)
    {
        if (_totalWaterCount >= 4)
        {
            objBottleControllerScript.NumberOfColorsInBottle = 4;
            _totalWaterCount -= 4;
        }
        else
        {
            objBottleControllerScript.NumberOfColorsInBottle = 0;
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

    private Color GetColorFromList(bool matchState, BottleController bottleController, int checkIndex)
    {
        if (_myColorsList.Count > 0)
        {
            int randomColorIndex = Random.Range(0, _myColorsList.Count);
            var color = _myColorsList[randomColorIndex].Color;

            if (checkIndex >= 0)
            {
                while (matchState && color == bottleController.GetColorAtPosition(checkIndex))
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
        _dispatcher.Invoke(() => SaveLevelAsPrefab());
    }
}