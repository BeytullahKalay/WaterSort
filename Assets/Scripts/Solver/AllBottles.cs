using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.iOS;
using Debug = UnityEngine.Debug;

public class AllBottles
{
    private List<Bottle> _allBottles = new List<Bottle>();

    private int _bottleIndex = 0;

    private int _maxIterationNum = 10000;
    private int _iterationNum = 0;

    // public AllBottles(List<BottleController> bottleControllers)
    // {
    //     foreach (var controller in bottleControllers)
    //     {
    //         Bottle b = new Bottle(_bottleIndex);
    //
    //         for (int i = 0; i < controller.NumberOfColorsInBottle; i++)
    //         {
    //             if (ColorNumerator.colorsNumerator.ContainsKey(controller.BottleColors[i]))
    //             {
    //                 b.NumberedBottleStack.Push(ColorNumerator.colorsNumerator[controller.BottleColors[i]]);
    //             }
    //         }
    //
    //         _allBottles.Add(b);
    //         b.CalculateTopColorAmount();
    //         _bottleIndex++;
    //     }
    // }

    public AllBottles(List<Bottle> tempBottles)
    {
        // foreach (var bottle in tempBottles)
        // {
        //     _allBottles.Add(bottle);
        //     bottle.CalculateTopColorAmount();
        //     _bottleIndex++;
        // }

        //_allBottles = new List<Bottle>(tempBottles);
        _allBottles = tempBottles.ToList();

        foreach (var bottle in _allBottles)
        {
            foreach (var color in bottle.BottleColorsHashCodes)
            {
                int blackHashCode = 532676608;
                if (color != blackHashCode)
                    bottle.NumberedBottleStack.Push(color);
            }
        }

        // Debug.Log(_allBottles.Capacity);
    }


    public bool IsSolvable()
    {
        //Debug.Log("All bottle length is: " + _allBottles.Count);

        if (CheckAllBottleSorted())
        {
            Debug.Log("sorted level");
            return false;
        }

        TrySort(null);
        Debug.Log("Iteration Num: " + _iterationNum);
        return CheckAllBottleSorted();
    }

    private void TrySort(TransferMoves comingMove)
    {
        if (_iterationNum > _maxIterationNum) return;


        _iterationNum++;

        for (int i = 0; i < _allBottles.Count; i++)
        {
            if (CheckAllBottleSorted()) return;

            var movesQueue = new Queue<TransferMoves>();

            for (int j = 0; j < _allBottles.Count; j++)
            {
                var move = new TransferMoves(_allBottles[i], _allBottles[j]);

                if (move.CheckCanTransfer())
                {
                    movesQueue.Enqueue(move);
                }
            }


            while (movesQueue.Count > 0)
            {
                var currentMove = movesQueue.Dequeue();
                currentMove.DoAction();

                TrySort(currentMove);
                if (CheckAllBottleSorted()) return;
            }
        }

        if (CheckAllBottleSorted()) return;


        if (comingMove != null)
        {
            comingMove.UndoActions();
        }
    }

    private bool CheckAllBottleSorted()
    {
        bool isAllSorted = true;

        //Debug.Log(_allBottles.Count);

        for (int i = 0; i < _allBottles.Count; i++)
        {
            //Debug.Log(_allBottles[i].NumberedBottleStack.Count);
            if (!_allBottles[i].GetSorted() && _allBottles[i].NumberedBottleStack.Count > 0)
            {
                isAllSorted = false;
                break;
            }
        }

        return isAllSorted;
    }
}