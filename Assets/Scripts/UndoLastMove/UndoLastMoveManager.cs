using System;
using System.Collections.Generic;
using System.Linq;
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

    private readonly List<Move> _moves = new List<Move>();

    private void Start()
    {
        EventManager.UpdateRemainingUndo(remainingUndo);

        _remainingUndoCounter = remainingUndo;
    }

    private void AddMoveToList(BottleController first, BottleController second, int numberOfTopColorLayer,Color color)
    {
        Move move = new Move(first, second, numberOfTopColorLayer,color);

        if (_moves.Count >= 5) _moves.RemoveAt(0);

        _moves.Add(move);
    }

    private void UndoLastMove()
    {
        if (_moves.Count <= 0 || _remainingUndoCounter <= 0)
        {
            print("no move in list or no remaining undo.");
            return;
        }

        var lastMove = _moves.Last();
        _moves.Remove(lastMove);

        lastMove.UndoMove();
        _remainingUndoCounter--;

        _remainingUndoCounter = (int)Mathf.Clamp(_remainingUndoCounter, 0, Mathf.Infinity);

        EventManager.UpdateRemainingUndo(_remainingUndoCounter);
    }
}

[Serializable]
class Move
{
    private BottleController _firstBottle;
    private BottleController _secondBottle;

    private int _numberOfTopColorLayer;

    private Color _color;

    public Move(BottleController first, BottleController second, int colorLayerAmount,Color color)
    {
        _firstBottle = first;
        _secondBottle = second;
        _numberOfTopColorLayer = colorLayerAmount;
        _color = color;
    }

    public void UndoMove()
    {
        _firstBottle.NumberOfColorsInBottle += _numberOfTopColorLayer;
        _secondBottle.NumberOfColorsInBottle -= _numberOfTopColorLayer;


        int firstStartIndex = _firstBottle.NumberOfColorsInBottle - _numberOfTopColorLayer;
        firstStartIndex = (int)Mathf.Clamp(firstStartIndex,0, Mathf.Infinity);
        
        for (int i = firstStartIndex; i < _firstBottle.NumberOfColorsInBottle; i++)
        {
            _firstBottle.BottleColors[i] = _color;
        }

        _firstBottle.UpdateAfterUndo();
        _secondBottle.UpdateAfterUndo();
    }
}