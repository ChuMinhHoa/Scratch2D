using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopDataSave : IDataSave<ShopDataSave>
{
    public static ShopDataSave Instance => InGameDataManager.Instance.InGameData.ShopDataSave;
    [field: SerializeField] public Reactive<bool> IsFirstPurchase { get; set; } = new(true);
    public bool IsDirty { get; set; }

    public ShopDataSave DefaultData()
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
}