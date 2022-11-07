using System.Collections.Generic;

public class Bottle
{
    public Stack<int> NumberedBottleStack = new Stack<int>();
    
    private int _bottleIndex =  -1;
    private int _topColorAmount;
    
    private bool _sorted;
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
}