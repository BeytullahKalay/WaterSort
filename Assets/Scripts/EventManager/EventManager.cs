using System;
using UnityEngine;

public static class EventManager
{
    public static Action CheckIsLevelCompleted;
    public static Action LevelCompleted;
    public static Action LoadNextLevel;
    public static Action<BottleController, BottleController,int,Color> AddMoveToList;
    public static Action UndoLastMove;
    public static Action<int> UpdateRemainingUndo;

}
