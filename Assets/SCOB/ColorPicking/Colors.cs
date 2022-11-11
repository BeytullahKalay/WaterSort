using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Objects/Colour Swatch Object")]
public class Colors : ScriptableObject
{
    private int _randomChanger;
    
    [System.Serializable]
    public class Entry
    {
        public string name;
        public Color color;
    }
    
    public List<Entry> colors = new List<Entry>();

    public Color GetRandomColor()
    {
        var hasString = "Level " + _randomChanger.ToString(); 
        var rand = new Unity.Mathematics.Random((uint)hasString.GetHashCode());
        _randomChanger += 7;
        return colors[rand.NextInt(0, colors.Count)].color;
        
        //return colors[Random.Range(0, colors.Count)].color;
    }
}
