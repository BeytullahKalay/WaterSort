using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Objects/Colour Swatch Object")]
public class Colors : ScriptableObject
{
    
    [System.Serializable]
    public class Entry
    {
        public string name;
        public Color color;
    }
    
    public List<Entry> colors = new List<Entry>();

    public Color GetRandomColor(int comingColorIndex)
    {
        var hashString = "GetRandomColor " + comingColorIndex.ToString(); 
        var rand = new Unity.Mathematics.Random((uint)hashString.GetHashCode());
        return colors[rand.NextInt(0, colors.Count)].color;
    }
}
