using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Level[] level_SO;

    private GameObject _currentLevel;

    private void Awake()
    {
        CheckPlayerPrefs();
    }

    private void OnEnable()
    {
        EventManager.LevelCompleted += IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel += CreateLevel;
        EventManager.RestartLevel += RestartLevel;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel -= CreateLevel;
        EventManager.RestartLevel -= RestartLevel;
    }

    private void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        if (PlayerPrefs.GetInt("LevelIndex") > level_SO.Length)
        {
            Debug.Log("Level manager could not create level because of there is no level in index " +
                      PlayerPrefs.GetInt("LevelIndex"));
        }

        _currentLevel = Instantiate(level_SO[PlayerPrefs.GetInt("LevelIndex")].LevelPrefab.gameObject);
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
}