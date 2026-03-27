using System;
using CoreData;
using UnityEngine;

[Serializable]
public class InGameData
{
    [field: SerializeField] public PlayerInfoDataSave PlayerInfoDataSave { get; set; } = new();
    [field: SerializeField] public PlayerResourceDataSave PlayerResourceDataSave { get; set; } = new();
    [field: SerializeField] public SettingDataSave SettingDataSave { get; set; } = new();
    [field: SerializeField] public ShopDataSave ShopDataSave { get; set; } = new();
    [field: SerializeField] public EnergyDataSave EnergyDataSave { get; set; } = new();
    [field: SerializeField] public TimeDataSave TimeDataSave { get; set; }
    [field: SerializeField] public TutorialDataSave TutorialDataSave { get; set; }

    public void LoadData()
    {
        PlayerInfoDataSave = DataSerializer.LoadDataFromPrefs<PlayerInfoDataSave>();
        PlayerResourceDataSave = DataSerializer.LoadDataFromPrefs<PlayerResourceDataSave>();
        SettingDataSave = DataSerializer.LoadDataFromPrefs<SettingDataSave>();
        ShopDataSave = DataSerializer.LoadDataFromPrefs<ShopDataSave>();
        EnergyDataSave = DataSerializer.LoadDataFromPrefs<EnergyDataSave>();
        TimeDataSave = DataSerializer.LoadDataFromPrefs<TimeDataSave>();
        TutorialDataSave = DataSerializer.LoadDataFromPrefs<TutorialDataSave>();
    }

    public void SaveAllData()
    {
        PlayerInfoDataSave.SaveData();
        PlayerResourceDataSave.SaveData();
        SettingDataSave.SaveData();
        ShopDataSave.SaveData();
        EnergyDataSave.SaveData();
        TimeDataSave.SaveData();
        TutorialDataSave.SaveData();
    }
}