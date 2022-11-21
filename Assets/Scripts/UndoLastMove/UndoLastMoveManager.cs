using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UndoLastMoveManager : MonoBehaviour
{


    [SerializeField] private int remainingUndo = 5;

    private int _remainingUndoCounter;

    [SerializeField] private List<Move> _moves = new List<Move>();
    
    private void OnEnable()
    {
        EventManager.AddMoveToList += AddMoveToList;
        EventManager.UndoLastMove += UndoLastMove;
        EventManager.RestartLevel += ResetUndoActions;
        EventManager.CreateLevel += ResetUndoActions;
        EventManager.ResetUndoActions += ResetUndoActions;
    }

    private void OnDisable()
    {
        EventManager.AddMoveToList -= AddMoveToList;
        EventManager.UndoLastMove -= UndoLastMove;
        EventManager.RestartLevel -= ResetUndoActions;
        EventManager.CreateLevel -= ResetUndoActions;
        EventManager.ResetUndoActions -= ResetUndoActions;
    }


    private void Start()
    {
        EventManager.UpdateRemainingUndo?.Invoke(remainingUndo);

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

    private void ResetUndoActions()
    {
        _moves.Clear();
        _remainingUndoCounter = remainingUndo;
        EventManager.UpdateRemainingUndo(remainingUndo);
    }
}

