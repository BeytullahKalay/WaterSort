using System;
using UnityEngine;

public static class EventManager
{
    public static Action CheckIsLevelCompleted;
    public static Action LevelCompleted;
    public static Action LoadNextLevel;
    public static Action DeletePlayedLevelAndCreateNewLevel;
    public static Action<BottleController, BottleController,int,Color> AddMoveToList;
    public static Action UndoLastMove;
    public static Action<int> UpdateRemainingUndo;
    public static Action RestartLevel;
    public static Action UpdateLevelText;
    public static Action CreateLevel;
    public static Action<Level> SaveLevel;
    public static Action AddExtraEmptyBottle;
    public static Action<bool> ButtonIntractable;
}
