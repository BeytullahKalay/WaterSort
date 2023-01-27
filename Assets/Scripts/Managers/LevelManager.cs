using System.Collections.Generic;
using System.IO;
using Solver;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelHolder levelHolder;

    private GameObject _currentLevel;


    private void Awake()
    {
        // Initialize new json path string list 
        levelHolder.JsonPathString = new List<string>();

        CheckPlayerPrefs();
    }

    private void OnEnable()
    {
        EventManager.LevelCompleted += IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel += LoadNextLevel;
        EventManager.RestartLevel += RestartLevel;
        EventManager.SaveJsonFilePath += SaveJsonFilePath;
        EventManager.GetLevelParent += GetLevelParent;
        EventManager.CreateNewLevelForJson += CreateNewLevelForJson;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel -= LoadNextLevel;
        EventManager.RestartLevel -= RestartLevel;
        EventManager.SaveJsonFilePath -= SaveJsonFilePath;
        EventManager.GetLevelParent -= GetLevelParent;
        EventManager.CreateNewLevelForJson -= CreateNewLevelForJson;
    }

    private void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        if (_currentLevel != null) Destroy(_currentLevel);

        levelHolder.GetJsonFile(Paths.LevelHolderPath);

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
        if (!PlayerPrefs.HasKey(PlayerPrefNames.LevelIndex))
            PlayerPrefs.SetInt(PlayerPrefNames.LevelIndex, 0);
    }

    private void IncreaseLevelIndexOnLevelCompleted()
    {
        PlayerPrefs.SetInt(PlayerPrefNames.LevelIndex, PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex) + 1);
    }

    private void RestartLevel()
    {
        Destroy(_currentLevel);
        CreateLevel();
    }

    private void SaveJsonFilePath(string s)
    {
        levelHolder.UpdateAndAdd(Paths.LevelHolderPath, s);
    }

    private void GetLevelParent(GameObject levelParent)
    {
        _currentLevel = levelParent;
    }

    private void LoadNextLevel()
    {
        levelHolder.UpdateAndRemoveAt(Paths.LevelHolderPath, 0);
        CreateLevel();
    }

    private void CreateNewLevelForJson()
    {
        EventManager.CreateLevel?.Invoke();
    }
}