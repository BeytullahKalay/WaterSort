using System.Collections.Generic;
using UnityEngine;

public class AllBottles
{
    private List<Bottle> _allBottles = new List<Bottle>();

    public AllBottles(List<BottleController> bottleControllers)
    {
        foreach (var controller in bottleControllers)
        {
            Bottle b = new Bottle();

            for (int i = 0; i < controller.NumberOfColorsInBottle; i++)
            {
                if (ColorNumerator.colorsNumerator.ContainsKey(controller.BottleColors[i]))
                {
                    b.NumberedBottleStack.Push(ColorNumerator.colorsNumerator[controller.BottleColors[i]]);
                }
            }
            _allBottles.Add(b);
        }
    }

    public bool IsSolvable()
    {

        TrySort(null);

        return CheckAllBottleSorted();
    }

    private void TrySort(TransferMoves comingMove)
    {
        for (int i = 0; i < _allBottles.Count; i++)
        {
            if (CheckAllBottleSorted()) return;

            Stack<TransferMoves> movesStack = new Stack<TransferMoves>();

            for (int j = 0; j < _allBottles.Count; j++)
            {
                var move = new TransferMoves(_allBottles[i], _allBottles[j]);

                if (move.CheckCanTransfer())
                {
                    movesStack.Push(move);
                }
            }


            while (movesStack.Count > 0)
            {
                var currentMove = movesStack.Pop();
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

        // TODO: Buraya birden fazla kez geliyor!!
    }

    private bool CheckAllBottleSorted()
    {
        bool isAllSorted = true;

        for (int i = 0; i < _allBottles.Count; i++)
        {
            if (!_allBottles[i].GetSorted())
                isAllSorted = false;
        }

        return isAllSorted;
    }
}
