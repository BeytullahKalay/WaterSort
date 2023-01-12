using System.IO;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    
    private void CheckPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefNames.FirstTime))
            PlayerPrefs.SetInt(PlayerPrefNames.FirstTime, 0);

        if (PlayerPrefs.GetInt(PlayerPrefNames.FirstTime) == 0)
            DeleteAllData();

        PlayerPrefs.SetInt(PlayerPrefNames.FirstTime, 20);
    }

    private void DeleteAllData()
    {
        PlayerPrefs.SetInt(PlayerPrefNames.NamingIndex, 0);
        PlayerPrefs.SetInt(PlayerPrefNames.LevelIndex, 0);
        EventManager.UpdateLevelText?.Invoke();

        // delete level creation data
        if (File.Exists(Paths.LevelCreationDataPath))
        {
            File.Delete(Paths.LevelCreationDataPath);
        }

        // delete level data
        var maxWaterAmountBottleCanTake = 4;
        for (int i = 0; i < maxWaterAmountBottleCanTake; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/" + i + "data.json"))
                File.Delete(Application.persistentDataPath + "/" + i + "data.json");
        }
        
        // delete level holder data
        if (File.Exists(Paths.LevelHolderPath))
        {
            File.Delete(Paths.LevelHolderPath);
        }
    }

    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        CheckPlayerPrefs();
    }
}