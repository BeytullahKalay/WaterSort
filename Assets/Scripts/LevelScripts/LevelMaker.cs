using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelMaker : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.LoadNextLevel += CreateNewLevel_GUIButton;
    }

    private void OnDisable()
    {
        EventManager.LoadNextLevel -= CreateNewLevel_GUIButton;
    }


    [Header("Bottle Sorting Values")]
    [SerializeField] [Range(0, 1)] private float bottleDistanceX = .01f;
    [SerializeField] [Range(0, 1)] private float bottleStartPosY = .75f;
    [SerializeField] [Range(0, 1)] private float bottleDistanceY = .01f;



    [SerializeField] private GameObject bottle;

    [Space(10)] [SerializeField] private LineRenderer lineRenderer;

    [Header("Color Database")] [SerializeField] private Colors _colorsdb;

    [Header("Level Maker")]
    [SerializeField] private int numberOfColorsToCreate = 2;
    [SerializeField] private List<Color> selectedColors = new List<Color>();
    private List<MyColors> _myColorsList = new List<MyColors>();

    private int _createdBottles;
    private int _numberOfBottlesCreate;
    private int _totalWaterCount;

    private GameObject _levelParent;
    private GameObject _line1;
    private GameObject _line2;
    private GameObject _obj;

    [Space(20)] [SerializeField] private GameObject lastCreatedParent;

    [Header("Created Bottles")] public List<BottleController> createdBottlesContainer;


    private void Start()
    {
        ColorNumerator.NumerateColors(selectedColors);
        AllBottles allBottles = new AllBottles(createdBottlesContainer);
        print("CREATED!");
        
        if (allBottles.IsSolvable())
        {
            print("Solvable!");
        }
        else
        {
            print("Not Solvable :(");
        }
    }

    // using by inspector gui
    public void CreateNewLevel_GUIButton()
    {
        createdBottlesContainer.Clear();
        DestroyImmediate(lastCreatedParent);
        CreateLevel();
    }

    private void CreateLevel()
    {
        _createdBottles = 0;
        selectedColors.Clear();
        
        SelectColorsToCreate();

        CreateColorObjects();

        _totalWaterCount = selectedColors.Count * 4;

        RandomizeNumberOfBottle();
        
        CreateLevelParentAndLineObjects();

        CreateBottles(_numberOfBottlesCreate);
    }

    private void RandomizeNumberOfBottle()
    {
        _numberOfBottlesCreate = Random.Range(numberOfColorsToCreate + 1, numberOfColorsToCreate + 3);
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


    private void CreateBottles(float num)
    {
        for (int i = 0; i < num; i++)
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

            var objBottleControllerScript = _obj.GetComponent<BottleController>();
            
            objBottleControllerScript.LineRenderer = lineRenderer;

            if (_totalWaterCount >= 4)
            {
                objBottleControllerScript.NumberOfColorsInBottle = 4;
                _totalWaterCount -= 4;
            }
            else
            {
                objBottleControllerScript.NumberOfColorsInBottle = 0; 
            }
            


            for (int j = 0; j < objBottleControllerScript.BottleColors.Length; j++)
            {
                objBottleControllerScript.BottleColors[j] = GetColorFromList();
            }

            var posA = new Vector3(_createdBottles % (num / 2) * bottleDistanceX,
                bottleStartPosY - bottleDistanceY * Mathf.Floor(_createdBottles / (num / 2)), 0);

            Vector3 pos = Camera.main.ViewportToWorldPoint(posA);
            pos.z = 0;

            _obj.transform.position = pos;

            _obj.transform.SetParent(_createdBottles < (num / 2) ? _line1.transform : _line2.transform);

            createdBottlesContainer.Add(_obj.GetComponent<BottleController>());
            
            _createdBottles++;
        }

        AlignBottles();
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

    private Color GetColorFromList()
    {
        if (_myColorsList.Count > 0)
        {
            int randomColorIndex = Random.Range(0, _myColorsList.Count);

            var color = _myColorsList[randomColorIndex].Color;

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
}

public class MyColors
{
    public Color Color;
    public int Amount = 0;

    public MyColors(Color color)
    {
        this.Color = color;
        this.Amount = 0;
    }

    public bool MoreThan4()
    {
        return Amount >= 4;
    }
}