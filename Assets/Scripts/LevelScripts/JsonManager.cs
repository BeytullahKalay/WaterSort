using System.IO;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    public class JsonManager
    {
        public static void TryGetLevelCreateDataFromJson(Data data)
        {
            string path = Paths.LevelCreationDataPath;

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<Data>(json);
            }
            else
            {
                Debug.Log("Data not exists. Creating new one");
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(path, json);
            }
        }
    
        public static void SaveToJson(AllBottles allBottles)
        {
            string json = JsonUtility.ToJson(allBottles);
            string path = Path.Combine(Application.persistentDataPath,
                PlayerPrefs.GetInt(PlayerPrefNames.NamingIndex) % 4 + "data.json");
            File.WriteAllText(path, json);
            EventManager.SaveJsonFilePath?.Invoke(path);
            PlayerPrefs.SetInt(PlayerPrefNames.NamingIndex, PlayerPrefs.GetInt(PlayerPrefNames.NamingIndex) + 1);
        }
    
        public static void SaveLevelCreateDataToJson(ref Data data)
        {
            string json = JsonUtility.ToJson(data);
            string path = Paths.LevelCreationDataPath;
            File.WriteAllText(path, json);
        }
    }
}
