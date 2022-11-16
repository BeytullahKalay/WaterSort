using UnityEngine;

[CreateAssetMenu(menuName = "Objects/Level",fileName = "Level_")]
public class Level : ScriptableObject
{
    public LevelParent LevelPrefab;

    public string PrefabPath;
    public string SCOB_Path;
}