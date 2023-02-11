using System.IO;
using DataRepo;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    public static class JsonManager
    {
        public static void TryGetLevelCreateDataFromJson(ref Data data)
        {
            var path = Paths.LevelCreationDataPath;

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                data = JsonUtility.FromJson<Data>(json);
            }
            else
            {
                Debug.Log("Data not exists. Creating new one");
                var json = JsonUtility.ToJson(data);
                File.WriteAllText(path, json);
            }
        }
    
        public static void SaveToJson(AllBottles allBottles)
        {
            var json = JsonUtility.ToJson(allBottles);
            var path = Path.Combine(Application.persistentDataPath,
                PlayerPrefs.GetInt(PlayerPrefNames.NamingIndex) % 4 + "data.json");
            File.WriteAllText(path, json);
            EventManager.SaveJsonFilePath?.Invoke(path);
            PlayerPrefs.SetInt(PlayerPrefNames.NamingIndex, PlayerPrefs.GetInt(PlayerPrefNames.NamingIndex) + 1);
        }
    
        public static void SaveLevelCreateDataToJson(ref Data data)
        {
            var json = JsonUtility.ToJson(data);
            var path = Paths.LevelCreationDataPath;
            File.WriteAllText(path, json);
        }
    }
}
