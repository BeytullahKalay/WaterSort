using System.Collections.Generic;
using UnityEngine;

public static class ColorNumerator
{
    public static Dictionary<Color, int> colorsNumerator = new Dictionary<Color, int>();

    public static void NumerateColors(List<Color> colors)
    {
        colorsNumerator.Clear();
        for (int i = 0; i < colors.Count; i++)
        {
            colorsNumerator.Add(colors[i], i);
        }
    }
}


