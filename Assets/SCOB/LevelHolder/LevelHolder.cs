using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objects/LevelHolder",fileName = "LevelHolder")]
public class LevelHolder : ScriptableObject
{
    public List<Level> Levels_SO = new List<Level>();
}
