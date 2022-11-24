using System.Collections.Generic;
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
    }

    private void OnDisable()
    {
        EventManager.RestartLevel -= Restart;
        EventManager.LoadNextLevel -= LoadNextLevel;
        EventManager.SaveJsonFilePath -= SaveJsonFilePath;
    }

    private void Awake()
    {
        CheckTutorial();
    }

    private bool CheckTutorial()
    {
        if (PlayerPrefs.GetInt("LevelIndex") > tutorialLevels.Count - 1)
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
        CheckIsGameHavePlayerPrefs();

        SpawnLevel();
        EventManager.CreateLevel?.Invoke();
    }

    private void CheckIsGameHavePlayerPrefs()
    {
        if (levelHolder.JsonPathString.Count <= 0 && PlayerPrefs.GetInt("LevelIndex") > 0 ||
            PlayerPrefs.GetInt("NamingIndex") > 0)
        {
            PlayerPrefs.SetInt("NamingIndex",0);
            PlayerPrefs.SetInt("LevelIndex", 0);
            EventManager.UpdateLevelText?.Invoke();
        }
    }

    private void SpawnLevel()
    {
        var spawnObj = tutorialLevels[PlayerPrefs.GetInt("LevelIndex")];
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
        IncreaseLevelIndex();

        CheckTutorial();

        if (!CheckTutorial()) return;

        SpawnLevel();
        EventManager.CreateLevel?.Invoke();
    }

    private void IncreaseLevelIndex()
    {
        PlayerPrefs.SetInt("LevelIndex", PlayerPrefs.GetInt("LevelIndex") + 1);
    }

    private void SaveJsonFilePath(string s)
    {
        levelHolder.JsonPathString.Add(s);
    }
}