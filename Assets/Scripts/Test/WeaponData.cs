
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string Name;
    public int Damage;
    public bool IsSorted;

    public List<GameObject> List = new List<GameObject>();
}
