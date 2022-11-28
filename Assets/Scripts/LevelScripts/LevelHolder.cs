using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LevelHolder
{
    public List<string> JsonPathString = new List<string>();

    public void GetJsonFile(string path)
    {
        LevelHolder levelHolder = new LevelHolder();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            levelHolder = JsonUtility.FromJson<LevelHolder>(json);
        }
        else
        {
            Debug.Log("Level Holder not exists. Creating new one");
            string json = JsonUtility.ToJson(levelHolder);
            File.WriteAllText(path,json);
        }

        JsonPathString = new List<string>(levelHolder.JsonPathString);
    }

    public void UpdateAndAdd(string path,string pathToAdd)
    {
        GetJsonFile(path);
        JsonPathString.Add(pathToAdd);
        SaveToJsonFile(path);
    }

    public void UpdateAndRemoveAt(string path, int index)
    {
        GetJsonFile(path);
        JsonPathString.RemoveAt(index);
        SaveToJsonFile(path);
    }

    private void SaveToJsonFile(string path)
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }
}