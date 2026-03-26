using System;
using UnityEngine;

[Serializable]
public class EnergyDataSave : IDataSave<EnergyDataSave>
{
    public static EnergyDataSave Instance => InGameDataManager.Instance.InGameData.EnergyDataSave;
    public bool IsDirty { get; set; }
    public EnergyDataSave DefaultData()
    {
        return this;
    }
    
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public ShopDataSave FromJson(string json)
    {
        return JsonUtility.FromJson<ShopDataSave>(json);
    }
    
    public int currentEnergy = -1;
    public Reactive<string> timeToAddEnergy = new("");
    public Reactive<string> timeToAddOneEnergy = new("");
    public Reactive<string> timeToEndInfiniteEnergy = new("");
}
