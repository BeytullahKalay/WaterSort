using Firebase.Analytics;
using UnityEngine;

public class FirebaseSetup : MonoBehaviour
{
    private Firebase.FirebaseApp app;

    // private void OnEnable()
    // {
    //     EventManager.AddExtraEmptyBottle += OnBottleTake;
    //     EventManager.RestartLevel += OnLevelRestart;
    //     EventManager.LevelCompleted += OnLevelCompleted;
    //     EventManager.LoadNextLevel += OnLevelStart;
    // }
    //
    // private void OnDisable()
    // {
    //     EventManager.AddExtraEmptyBottle -= OnBottleTake;
    //     EventManager.RestartLevel -= OnLevelRestart;
    //     EventManager.LevelCompleted -= OnLevelCompleted;
    //     EventManager.LoadNextLevel -= OnLevelStart;
    // }

    #region Initialization

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    #endregion



    // // Analytics Events
    // private void OnLevelStart()
    // {
    //     FirebaseAnalytics.LogEvent("LEVEL START", "level_start", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex));
    // }
    //
    // private void OnLevelCompleted()
    // {
    //     FirebaseAnalytics.LogEvent("LEVEL COMPLETED", "level_completed",
    //         PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex));
    // }
    //
    // private void OnLevelRestart()
    // {
    //     FirebaseAnalytics.LogEvent("LEVEL RESTART", "level_restart", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex));
    // }
    //
    // private void OnBottleTake()
    // {
    //     FirebaseAnalytics.LogEvent("BOTTLE TAKE", "on_bottle_take", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex));
    // }
}