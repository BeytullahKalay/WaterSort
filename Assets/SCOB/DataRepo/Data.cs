using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objects/DataRepo")]
public class Data : ScriptableObject
{
    public int BottleColorRandomIndex = 0;
    public int ExtraBottleAmountRandomIndex = 0;
    public int ColorPickerRandomIndex = 0;

    [SerializeField] public List<Bottle> CreatedBottles = new List<Bottle>();


    public int GetBottleColorRandomIndex()
    {
        BottleColorRandomIndex += 50;
        return BottleColorRandomIndex - 50;
    }

    public int GetAmountOfExtraBottleIndex()
    {
        ExtraBottleAmountRandomIndex += 50;
        return ExtraBottleAmountRandomIndex - 50;
    }

    public int GetColorPickerRandomIndex()
    {
        ColorPickerRandomIndex +=50;
        return ColorPickerRandomIndex - 50;
    }
}