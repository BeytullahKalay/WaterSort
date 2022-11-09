using UnityEditor;
using UnityEngine;

public class LevelSaver : MonoBehaviour
{
    [SerializeField] private GameObject level;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveLevelAsPrefab();
            print("Saved");
        }
    }

    private void SaveLevelAsPrefab()
    {
        string localPath = "Assets/Prefabs/Levels/" + "Level" + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        PrefabUtility.SaveAsPrefabAssetAndConnect(level, localPath,InteractionMode.UserAction);
    }
}