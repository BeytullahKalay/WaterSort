using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TutorialLevels : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    [SerializeField] private List<GameObject> tutorialLevels = new List<GameObject>();

    [SerializeField] private LevelHolder levelHolder;

    private GameObject _currentLevel;

    private bool _isTutorialEnd;

    private void OnEnable()
    {
        EventManager.RestartLevel += Restart;
        EventManager.LoadNextLevel += LoadNextLevel;
        EventManager.SaveJsonFilePath += SaveJsonFilePath;
        EventManager.LevelCompleted += IncreaseLevelIndex;
    }

    private void OnDisable()
    {
        EventManager.RestartLevel -= Restart;
        EventManager.LoadNextLevel -= LoadNextLevel;
        EventManager.SaveJsonFilePath -= SaveJsonFilePath;
        EventManager.LevelCompleted -= IncreaseLevelIndex;
    }

    private void Awake()
    {
        CheckTutorial();
    }

    private bool CheckTutorial()
    {
        
        if (PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex) > tutorialLevels.Count - 1)
        {
            _isTutorialEnd = true;

            levelManager.gameObject.SetActive(true);

            EventManager.ResetUndoActions?.Invoke();

            Destroy(gameObject);
            return false;
        }

        return true;
    }

    private void Start()
    {
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        var spawnObj = tutorialLevels[PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex)];
        _currentLevel = Instantiate(spawnObj, spawnObj.transform.position, spawnObj.transform.rotation);
    }

    private void Restart()
    {
        if (_isTutorialEnd) return;

        Destroy(_currentLevel);
        SpawnLevel();
    }

    private void LoadNextLevel()
    {
        Destroy(_currentLevel);

        CheckTutorial();

        if (!CheckTutorial()) return;

        SpawnLevel();
        EventManager.CreateLevel?.Invoke();
    }

    private void IncreaseLevelIndex()
    {
        PlayerPrefs.SetInt(PlayerPrefNames.LevelIndex, PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex) + 1);
    }

    private void SaveJsonFilePath(string s)
    {
        
        levelHolder.UpdateAndAdd(Paths.LevelHolderPath,s);

        string path = Paths.LevelHolderPath;

        if (File.Exists(path))
        {
            string json = JsonUtility.ToJson(levelHolder);
            File.WriteAllText(path,json);
        }
        else
        {
            Debug.Log("no level holder data in json. Creating");

            string json = JsonUtility.ToJson(levelHolder);
            File.WriteAllText(path,json);
        }
    }
}