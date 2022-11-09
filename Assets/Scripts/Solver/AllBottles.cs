using System.Collections.Generic;
using UnityEngine;

public class AllBottles
{
    private List<Bottle> _allBottles = new List<Bottle>();

    private int _bottleIndex = 0;

    private int _maxTryAmount = 10000;
    private int _tryAmount = 0;

    public AllBottles(List<BottleController> bottleControllers)
    {
        foreach (var controller in bottleControllers)
        {
            Bottle b = new Bottle(_bottleIndex);

            for (int i = 0; i < controller.NumberOfColorsInBottle; i++)
            {
                if (ColorNumerator.colorsNumerator.ContainsKey(controller.BottleColors[i]))
                {
                    b.NumberedBottleStack.Push(ColorNumerator.colorsNumerator[controller.BottleColors[i]]);
                }
            }

            _allBottles.Add(b);
            b.CalculateTopColorAmount();
            _bottleIndex++;
        }
    }


    public bool IsSolvable()
    {
        TrySort(null);
        Debug.Log("Try Amount: " + _tryAmount);
        return CheckAllBottleSorted();
    }

    private void TrySort(TransferMoves comingMove)
    {
        if(_tryAmount > _maxTryAmount) return;
        
        
        _tryAmount++;

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

        for (int i = 0; i < _allBottles.Count; i++)
        {
            if (!_allBottles[i].GetSorted() && _allBottles[i].NumberedBottleStack.Count > 0)
            {
                isAllSorted = false;
                break;
            }
        }

        return isAllSorted;
    }
}