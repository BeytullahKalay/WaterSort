using System.Collections.Generic;

public class Bottle
{
    public Stack<int> NumberedBottleStack = new Stack<int>();
    private bool _sorted;

    public int GetTopColorID()
    {
        if (NumberedBottleStack.Count > 0)
            return NumberedBottleStack.Peek();
        else
        {
            //Debug.LogError("Bottle empty but trying get top color ID!");
            return -1;
        }
    }

    public int GetTopColorAmount()
    {
        if (NumberedBottleStack.Count <= 0) return 0;
        
        var stack = new Stack<int>(new Stack<int>(NumberedBottleStack));
        
        int firstColorNum = stack.Peek();
        int topColorAmount = 0;


        while (stack.Count > 0)
        {
            if (stack.Pop() == firstColorNum)
                topColorAmount++;
            else
                break;
        }
        
        

        return topColorAmount;
    }

    public void CheckIsSorted()
    {
        if (NumberedBottleStack.Count <= 0)
        {
            _sorted = true;
            return;
        }

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
}