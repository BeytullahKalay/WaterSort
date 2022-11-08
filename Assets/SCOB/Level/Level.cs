using UnityEngine;

[CreateAssetMenu(menuName = "Objects/Level",fileName = "Level_")]
public class Level : ScriptableObject
{
    [SerializeField] private LevelParent levelPrefab;

    public GameObject GetLevel()
    {
        return levelPrefab.gameObject;
    }
}