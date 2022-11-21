using System;
using UnityEngine;

public static class EventManager
{
    public static Action CheckIsLevelCompleted;
    public static Action LevelCompleted;
    public static Action<AllBottles> CreatePrototype;
    public static Action<GameObject> GetLevelParent;
    public static Action LoadNextLevel;
    public static Action DeletePlayedLevelAndCreateNewLevel;
    public static Action<BottleController, BottleController,int,Color> AddMoveToList;
    public static Action UndoLastMove;
    public static Action<int> UpdateRemainingUndo;
    public static Action RestartLevel;
    public static Action UpdateLevelText;
    public static Action CreateLevel;
    public static Action AddExtraEmptyBottle;
    public static Action<bool> ButtonIntractable;
    public static Action<string> SaveJsonFilePath;
}
