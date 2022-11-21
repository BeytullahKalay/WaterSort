using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelMaker))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        LevelMaker levelMaker = (LevelMaker)target;
        
        if (GUILayout.Button("Create New Level"))
        {
            levelMaker.CreateNewLevel_GUIButton();
        }
    }
}
