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
        BottleColorRandomIndex += 5;
        return BottleColorRandomIndex - 5;
    }

    public int GetAmountOfExtraBottleIndex()
    {
        ExtraBottleAmountRandomIndex += 5;
        return ExtraBottleAmountRandomIndex - 5;
    }

    public int GetColorPickerRandomIndex()
    {
        ColorPickerRandomIndex +=5;
        return ColorPickerRandomIndex - 5;
    }
}