using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Data
{
    public long BottleColorRandomIndex = 0;
    public long ExtraBottleAmountRandomIndex = 0;
    public long ColorPickerRandomIndex = 0;
    
    [SerializeField] public List<Bottle> CreatedBottles = new List<Bottle>();
    
    public long GetBottleColorRandomIndex()
    {
        BottleColorRandomIndex += 50;
        return BottleColorRandomIndex - 50;
    }
    
    public long GetAmountOfExtraBottleIndex()
    {
        ExtraBottleAmountRandomIndex += 50;
        return ExtraBottleAmountRandomIndex - 50;
    }

    public long GetColorPickerRandomIndex()
    {
        ColorPickerRandomIndex +=50;
        return ColorPickerRandomIndex - 50;
    }
}