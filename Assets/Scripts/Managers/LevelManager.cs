using UnityEditor;
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
        EventManager.LoadNextLevel += CreateLevel;
        EventManager.RestartLevel += RestartLevel;
        EventManager.DeletePlayedLevelAndCreateNewLevel += DeletePlayedLevelFromAssets;
        EventManager.DeletePlayedLevelAndCreateNewLevel += CreateNewLevelForAssets;
        EventManager.SaveLevel += AddLevelToLevelHolder;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= IncreaseLevelIndexOnLevelCompleted;
        EventManager.LoadNextLevel -= CreateLevel;
        EventManager.RestartLevel -= RestartLevel;
        EventManager.DeletePlayedLevelAndCreateNewLevel -= DeletePlayedLevelFromAssets;
        EventManager.DeletePlayedLevelAndCreateNewLevel -= CreateNewLevelForAssets;
        EventManager.SaveLevel -= AddLevelToLevelHolder;
    }

    private void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        if (_currentLevel != null) Destroy(_currentLevel);

        if (levelHolder.Levels_SO.Count > 0)
        {
            _currentLevel = Instantiate(levelHolder.Levels_SO[0].LevelPrefab.gameObject);
            
            EventManager.ButtonIntractable?.Invoke(_currentLevel.GetComponent<LevelParent>().isBottleAdded);
        }
        else
        {
            Debug.LogWarning("There is no level in Level_SO");
            return;
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

    private void DeletePlayedLevelFromAssets()
    {
        var levelToBeDeleted = levelHolder.Levels_SO[0];
        AssetDatabase.DeleteAsset(levelToBeDeleted.PrefabPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        AssetDatabase.DeleteAsset(levelToBeDeleted.SCOB_Path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        levelHolder.Levels_SO.Remove(levelToBeDeleted);
    }

    private void CreateNewLevelForAssets()
    {
        EventManager.CreateLevel?.Invoke();
    }

    private void AddLevelToLevelHolder(Level levelToBeSaved)
    {
        levelHolder.Levels_SO.Add(levelToBeSaved);
        EditorUtility.SetDirty(levelHolder);
    }
}