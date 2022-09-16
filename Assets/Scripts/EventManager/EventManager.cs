using System;

public static class EventManager
{
    public static Action CheckIsLevelCompleted;
    public static Action LevelCompleted;
    public static Action LoadNextLevel;
    public static Action<BottleController, BottleController,int> AddMoveToList;
    public static Action UndoLastMove;
    public static Action<int> UpdateRemainingUndo;

}
