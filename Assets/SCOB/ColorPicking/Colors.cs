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

    public Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Count)].color;
    }
}
