using UnityEngine;

[CreateAssetMenu(menuName = "Objects/DataRepo")]
public class Data : ScriptableObject
{
    public int BottleColorRandomIndex = 0;
    public int ExtraBottleAmountRandomIndex = 0;
    public int ColorPickerRandomIndex = 0;
    

    public int GetBottleColorRandomIndex()
    {
        BottleColorRandomIndex++;
        return BottleColorRandomIndex - 1;
    }

    public int GetAmountOfExtraBottleIndex()
    {
        ExtraBottleAmountRandomIndex++;
        return ExtraBottleAmountRandomIndex - 1;
    }

    public int GetColorPickerRandomIndex()
    {
        ColorPickerRandomIndex++;
        return ColorPickerRandomIndex - 1;
    }
}