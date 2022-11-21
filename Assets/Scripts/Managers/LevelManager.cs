using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelHolder levelHolder;

    private GameObject _currentLevel;


    private void Awake()
    {
        CheckPlayerPrefs();
    }

    private void OnEnable()
    {
        EventManager.LevelCompleted += IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel += LoadNextLevel;
        EventManager.RestartLevel += RestartLevel;
        EventManager.DeletePlayedLevelAndCreateNewLevel += CreateNewLevelForAssets;
        EventManager.SaveJsonFilePath += SaveJsonFilePath;
        EventManager.GetLevelParent += GetLevelParent;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel -= LoadNextLevel;
        EventManager.RestartLevel -= RestartLevel;
        EventManager.DeletePlayedLevelAndCreateNewLevel -= CreateNewLevelForAssets;
        EventManager.SaveJsonFilePath -= SaveJsonFilePath;
        EventManager.GetLevelParent -= GetLevelParent;
    }

    private void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        if (_currentLevel != null) Destroy(_currentLevel);

        if (levelHolder.JsonPathString.Count > 0)
        {
            string json = File.ReadAllText(levelHolder.JsonPathString[0]);
            AllBottles levelPrototype = JsonUtility.FromJson<AllBottles>(json);
            EventManager.CreatePrototype?.Invoke(levelPrototype);
        }
        else
        {
            Debug.LogWarning("There is no level in Level_SO");
        }
    }

    private void CheckPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("LevelIndex"))
            PlayerPrefs.SetInt("LevelIndex", 0);
    }

    private void IncreaseLevelIndexOnLevelCompleted()
    {
        PlayerPrefs.SetInt("LevelIndex", PlayerPrefs.GetInt("LevelIndex") + 1);
    }

    private void RestartLevel()
    {
        Destroy(_currentLevel);
        CreateLevel();
    }

    private void CreateNewLevelForAssets()
    {
        EventManager.CreateLevel?.Invoke();
    }

    private void SaveJsonFilePath(string s)
    {
        levelHolder.JsonPathString.Add(s);
    }

    private void GetLevelParent(GameObject levelParent)
    {
        _currentLevel = levelParent;
    }

    private void LoadNextLevel()
    {
        levelHolder.JsonPathString.RemoveAt(0);
        CreateLevel();
    }
}