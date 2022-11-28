using System.IO;
using UnityEngine;

public static class Paths
{
    public static string LevelHolderPath =
        Path.Combine(Application.persistentDataPath, PlayerPrefNames.LevelHolder + ".json");

    public static string LevelCreationDataPath =
        Path.Combine(Application.persistentDataPath, PlayerPrefNames.LevelCreationData + ".json");
}