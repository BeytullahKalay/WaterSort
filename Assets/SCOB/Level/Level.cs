using UnityEngine;

[CreateAssetMenu(menuName = "Objects/Level",fileName = "Level_")]
public class Level : ScriptableObject
{
    [SerializeField] public LevelParent LevelPrefab;
}