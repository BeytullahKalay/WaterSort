using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Objects/DataRepo")]
public class Data : ScriptableObject
{
    public long BottleColorRandomIndex = 0;
    public long ExtraBottleAmountRandomIndex = 0;
    public long ColorPickerRandomIndex = 0;

    [SerializeField] public List<Bottle> CreatedBottles = new List<Bottle>();


    public long GetBottleColorRandomIndex()
    {
        BottleColorRandomIndex += 50;
        SetDirty();
        return BottleColorRandomIndex - 50;
    }



    public long GetAmountOfExtraBottleIndex()
    {
        ExtraBottleAmountRandomIndex += 50;
        SetDirty();
        return ExtraBottleAmountRandomIndex - 50;
    }

    public long GetColorPickerRandomIndex()
    {
        ColorPickerRandomIndex +=50;
        SetDirty();
        return ColorPickerRandomIndex - 50;
    }
    
    private void SetDirty()
    {
        Dispatcher.Instance.Invoke(() => { EditorUtility.SetDirty(this); });
    }
}