using System.Collections.Generic;
using UnityEngine;

public class LevelMaker : MonoBehaviour
{
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
    private List<int> _selectedColorsAmounts = new List<int>();

    private int _createdBottles;
    private int _bottleLineMax = 6;
    private int _numberOfBottlesCreate;
    private int _totalWaterCount;

    private GameObject _levelParent;
    private GameObject _line1;
    private GameObject _line2;
    private GameObject _obj;




    private void Start()
    {
        SelectColorsToCreate();

        SetColorsAmountArray();

        _totalWaterCount = selectedColors.Count * 4;

        RandomizeNumberOfBottle();
        
        CreateLevelParentAndLineObjects();

        CreateBottles(_numberOfBottlesCreate);
    }

    private void SetColorsAmountArray()
    {
        for (int i = 0; i < selectedColors.Count; i++)
        {
            _selectedColorsAmounts.Add(0);
        }
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

    private void CreateLevelParentAndLineObjects()
    {
        _levelParent = new GameObject("LevelParent");
        _line1 = new GameObject("Line1");
        _line2 = new GameObject("Line2");
        _line2.transform.parent = _line1.transform.parent = _levelParent.transform;
    }


    private void CreateBottles(float num)
    {
        for (int i = 0; i < num; i++)
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

            var objBottleControllerScript = _obj.GetComponent<MyBottleController>();
            
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
        // int randomColorIndex = Random.Range(0, selectedColors.Count);
        //
        // while (_selectedColorsAmounts[randomColorIndex] >= 4)
        // {
        //     randomColorIndex = Random.Range(0, selectedColors.Count);
        // }
        //
        // var c = selectedColors[randomColorIndex];
        //
        // _selectedColorsAmounts[randomColorIndex]++;
        //
        // return c;
        
        int randomColorIndex = Random.Range(0, selectedColors.Count);
        
        // print("in");
        //
        // while (_selectedColorsAmounts[randomColorIndex] >= 4)
        // {
        //     randomColorIndex = Random.Range(0, selectedColors.Count);
        // }
        //
        // print("out");


        _selectedColorsAmounts[randomColorIndex] += 1;


        return selectedColors[randomColorIndex];
    }
}