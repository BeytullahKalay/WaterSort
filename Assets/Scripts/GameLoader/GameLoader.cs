using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private LevelHolder levelHolder;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Debug.Log("Level holder json Count : " + levelHolder.JsonPathString.Count);
        Debug.Log("Level Index : " + PlayerPrefs.GetInt("LevelIndex"));
        Debug.Log("Naming Index : " + PlayerPrefs.GetInt("NamingIndex"));

        // check level prefs
        if (levelHolder.JsonPathString.Count <= 0 && (PlayerPrefs.GetInt("LevelIndex") > 0 ||
                                                      PlayerPrefs.GetInt("NamingIndex") > 0))
        {
            PlayerPrefs.SetInt("NamingIndex", 0);
            PlayerPrefs.SetInt("LevelIndex", 0);
            EventManager.UpdateLevelText?.Invoke();
        }

        // Load next Level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}