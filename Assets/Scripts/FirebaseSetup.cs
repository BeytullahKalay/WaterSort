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

    // private void OnDisable()
    // {
    //     EventManager.AddExtraEmptyBottle -= OnBottleTake;
    //     EventManager.RestartLevel -= OnLevelRestart;
    //     EventManager.LevelCompleted -= OnLevelCompleted;
    //     EventManager.LoadNextLevel -= OnLevelStart;
    // }

    #region Initialize Firebase

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


    // Analytics Events
    // private void OnLevelStart()
    // {
    //     FirebaseAnalytics.LogEvent("level_start",
    //         new Parameter("level_start", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex)));
    // }
    //
    // private void OnLevelCompleted()
    // {
    //     FirebaseAnalytics.LogEvent("level_completed", new Parameter("level_completed",
    //         PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex)));
    // }
    //
    // private void OnLevelRestart()
    // {
    //     FirebaseAnalytics.LogEvent("level_restart",
    //         new Parameter("level_restart", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex)));
    // }
    //
    // private void OnBottleTake()
    // {
    //     FirebaseAnalytics.LogEvent("bottle_take",
    //         new Parameter("on_bottle_take", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex)));
    // }

    // private void TEST()
    // {
    //     Firebase.Analytics.FirebaseAnalytics
    //         .LogEvent("on_bottle_take", "bottle_take", PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex));
    // }
}