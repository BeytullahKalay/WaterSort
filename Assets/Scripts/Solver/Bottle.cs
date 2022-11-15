using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bottle
{
    public Stack<int> NumberedBottleStack = new Stack<int>();

    public int ParentNum;
    public int NumberOfColorsInBottle = 0;

    public int[] BottleColorsHashCodes = new int[4];

    public Color[] BottleColors = new Color[4];

    private int _bottleIndex = -1;
    private int _topColorAmount;

    private Vector3 _openPosition;

    [SerializeField] private bool _sorted = false;

    public Bottle(int bottleIndex)
    {
        _bottleIndex = bottleIndex;
    }

    public int GetTopColorID()
    {
        if (NumberedBottleStack.Count > 0)
            return NumberedBottleStack.Peek();
        else
        {
            return -1;
        }
    }

    public void CalculateTopColorAmount()
    {
        if (NumberedBottleStack.Count <= 0)
        {
            _topColorAmount = 0;
            return;
        }

        var stack = new Stack<int>(new Stack<int>(NumberedBottleStack));

        int firstColorNum = stack.Peek();
        _topColorAmount = 0;


        while (stack.Count > 0)
        {
            if (stack.Pop() == firstColorNum)
                _topColorAmount++;
            else
                break;
        }
    }

    public int GetTopColorAmount()
    {
        return _topColorAmount;
    }


    public void CheckIsSorted()
    {
        if (NumberedBottleStack.Count < 4)
        {
            _sorted = false;
            return;
        }


        var stack = new Stack<int>(new Stack<int>(NumberedBottleStack));

        int topColorID = stack.Peek();

        while (stack.Count > 0)
        {
            if (stack.Pop() != topColorID)
            {
                _sorted = false;
                return;
            }
        }

        _sorted = true;
    }

    public bool GetSorted()
    {
        return _sorted;
    }

    public int GetBottleIndex()
    {
        return _bottleIndex;
    }

    public int GetColorHashCodeAtPosition(int checkIndex)
    {
        return BottleColorsHashCodes[checkIndex].GetHashCode();
    }

    public void FindPositionAndAssignToPos(float numberOfBottleToCreate, int createdBottles, float bottleDistanceX,
        float bottleStartPosY, float bottleDistanceY)
    {
        var posA = new Vector3(createdBottles % (numberOfBottleToCreate / 2) * bottleDistanceX,
            bottleStartPosY - bottleDistanceY * Mathf.Floor(createdBottles / (numberOfBottleToCreate / 2)), 0);

        Vector3 pos = Camera.main.ViewportToWorldPoint(posA);
        pos.z = 0;

        _openPosition = pos;
    }

    public Vector3 GetOpenPosition()
    {
        return _openPosition;
    }
}