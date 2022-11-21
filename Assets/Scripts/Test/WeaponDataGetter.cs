using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponDataGetter : MonoBehaviour
{
    public string Name;
    public int Damage;
    public bool IsSorted;
    public List<GameObject> List = new List<GameObject>();

    private void SaveToJson()
    {
        WeaponData wpData = new WeaponData();

        wpData.Damage = this.Damage;
        wpData.Name = this.Name;
        wpData.IsSorted = this.IsSorted;
        wpData.List = this.List;


        string json = JsonUtility.ToJson(wpData);
        File.WriteAllText(Application.dataPath + "/WeaponDataFile.json",json);
    }
    
    private void LoadFromJson()
    {
        string json = File.ReadAllText(Application.dataPath + "/WeaponDataFile.json");
        WeaponData data = JsonUtility.FromJson<WeaponData>(json);

        Name = data.Name;
        Damage = data.Damage;
        IsSorted = data.IsSorted;
        List = data.List;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            print("Saved");
            SaveToJson();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            print("Loaded");
            LoadFromJson();
        }
    }
}
