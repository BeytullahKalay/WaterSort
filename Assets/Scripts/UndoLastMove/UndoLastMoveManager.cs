using System;
using System.Collections.Generic;
using UnityEngine;

public class UndoLastMoveManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.AddMoveToList += AddMoveToList;
        EventManager.UndoLastMove += UndoLastMove;
    }
    private void OnDisable()
    {
        EventManager.AddMoveToList -= AddMoveToList;
        EventManager.UndoLastMove -= UndoLastMove;
    }


    [SerializeField] private int remainingUndo = 5;

    private int _remainingUndoCounter;

    private readonly Stack<Move> _moves = new Stack<Move>();

    private void Start()
    {
        EventManager.UpdateRemainingUndo(remainingUndo);

        _remainingUndoCounter = remainingUndo;
    }

    private void AddMoveToList(BottleController first, BottleController second, int numberOfTopColorLayer)
    {
        Move move = new Move(first, second, numberOfTopColorLayer);

        if (_moves.Count >= 5)
            _moves.Pop();

        _moves.Push(move);
    }

    private void UndoLastMove()
    {
        if (_moves.Count <= 0 || _remainingUndoCounter <= 0)
        {
            print("no move in list or no remaining undo.");
            return;
        }

        var lastMove = _moves.Pop();
        lastMove.UndoMove();
        _remainingUndoCounter--;

        EventManager.UpdateRemainingUndo(_remainingUndoCounter);
    }
}

[Serializable]
class Move
{
    private BottleController _firstBottle;
    private BottleController _secondBottle;

    private int _numberOfTopColorLayer;

    public Move(BottleController first, BottleController second, int colorLayerAmount)
    {
        _firstBottle = first;
        _secondBottle = second;
        _numberOfTopColorLayer = colorLayerAmount;
    }

    public void UndoMove()
    {
        _firstBottle.NumberOfColorsInBottle += _numberOfTopColorLayer;
        _firstBottle.UpdateAfterUndo();

        _secondBottle.NumberOfColorsInBottle -= _numberOfTopColorLayer;
        _secondBottle.UpdateAfterUndo();
    }
}