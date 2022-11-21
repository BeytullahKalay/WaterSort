using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class AllBottles
{
    public List<Bottle> _allBottles = new List<Bottle>();
    
    private int _maxIterationNum = 10000;
    private int _iterationNum = 0;

    public AllBottles(List<Bottle> tempBottles)
    {
        _allBottles = tempBottles.ToList();
    }


    public bool IsSolvable()
    {
        if (CheckIsOneBottleSorted())
        {
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

    private bool CheckIsOneBottleSorted()
    {
        bool isOnBottleSorted = false;

        for (int i = 0; i < _allBottles.Count; i++)
        {
            if (_allBottles[i].GetSorted() && _allBottles[i].NumberedBottleStack.Count > 0)
            {
                isOnBottleSorted = true;
                break;
            }
        }

        return isOnBottleSorted;
    }
}