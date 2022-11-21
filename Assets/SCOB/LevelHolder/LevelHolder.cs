using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objects/LevelHolder",fileName = "LevelHolder")]
public class LevelHolder : ScriptableObject
{
    public List<string> JsonPathString = new List<string>();
}
