using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class EnergyManager : Singleton<EnergyManager>
{
    public GameResource energyResource;
    public Reactive<string> timeToAddAllEnergy = new("");
    public Reactive<string> timeToAddOneEnergy = new("");

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        energyResource = new GameResource(GameResource.Type.Energy, EnergyDataSave.Instance.currentEnergy);
        timeToAddAllEnergy = EnergyDataSave.Instance.timeToAddEnergy;
        timeToAddOneEnergy = EnergyDataSave.Instance.timeToAddOneEnergy;
        if (energyResource.Amount == -1)
        {
            FirstTimeInit();
        }

        if (!timeToAddAllEnergy.Value.Equals(""))
        {
            CheckTimeAddAllEnergy();
        }

        if (!timeToAddOneEnergy.Value.Equals(""))
        {
            CheckTimeAddOneEnergy();
        }
    }

    private void CheckTimeAddOneEnergy()
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var nextEnergyAddTime = timeToAddOneEnergy.Value.ToDateTime();

        if (nextEnergyAddTime.Subtract(currentTime).TotalSeconds > 0)
        {
            TimeManager.Instance.RegisterEventTime(nextEnergyAddTime, AddOneEnergy);
            return;
        }

        AddOneEnergy();
    }

    private void CheckTimeAddAllEnergy()
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var nextEnergyAddTime = timeToAddAllEnergy.Value.ToDateTime();

        if (nextEnergyAddTime.Subtract(currentTime).TotalSeconds > 0) return;

        AddEnergy((DefaultGlobalConfig.Instance.maxEnergy - energyResource.Amount).ToInt());
        ResetTimeToAddEnergy();
        ResetTimeToAddOneEnergy();
    }

    private void ResetTimeToAddEnergy()
    {
        timeToAddAllEnergy.Value = "";
        EnergyDataSave.Instance.SaveData();
    }

    private void ResetTimeToAddOneEnergy()
    {
        timeToAddOneEnergy.Value = "";
        EnergyDataSave.Instance.SaveData();
    }

    private void FirstTimeInit()
    {
        energyResource.Amount = DefaultGlobalConfig.Instance.maxEnergy;
        SaveEnergyData();
    }

    private void SaveEnergyData()
    {
        EnergyDataSave.Instance.currentEnergy = energyResource.Amount.ToInt();
        EnergyDataSave.Instance.SaveData();
    }

    [Button]
    public void UseEnergy(int amount)
    {
        energyResource.Amount -= amount;
        SaveEnergyData();
        if (energyResource.Amount < DefaultGlobalConfig.Instance.maxEnergy)
        {
            SaveTimeToAddAllEnergy();
            if (!timeToAddOneEnergy.Value.Equals(""))
                return;
            SaveTimeToAddOneEnergy();
        }
    }

    private void SaveTimeToAddOneEnergy()
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var defaultMinutesForEnergy = DefaultGlobalConfig.Instance.defaultMinutesForEnergy;
        var timeAddEnergyString = timeToAddOneEnergy.Value.Equals("")
            ? currentTime.AddMinutes(defaultMinutesForEnergy).ToEnUsString() :
        timeToAddOneEnergy.Value.ToDateTime().AddMinutes(defaultMinutesForEnergy).ToEnUsString();
        timeToAddOneEnergy.Value = timeAddEnergyString;
        TimeManager.Instance.RegisterEventTime(timeAddEnergyString.ToDateTime(), AddOneEnergy);
        EnergyDataSave.Instance.SaveData();
    }

    private void SaveTimeToAddAllEnergy()
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var defaultMinutesForEnergy = DefaultGlobalConfig.Instance.defaultMinutesForEnergy;
        var nextEnergyAddTime = timeToAddAllEnergy.Value.Equals("")
            ? currentTime.AddMinutes(defaultMinutesForEnergy)
            : timeToAddAllEnergy.Value.ToDateTime().AddMinutes(defaultMinutesForEnergy);

        var timeToAddEnergyString = nextEnergyAddTime.ToEnUsString();
        timeToAddAllEnergy.Value = timeToAddEnergyString;
        EnergyDataSave.Instance.SaveData();
    }

    public void AddOneEnergy()
    {
        AddEnergy(1);
    }

    private void AddEnergy(int amount)
    {
        energyResource.Amount += amount;
        SaveEnergyData();
        if (energyResource.Amount >= DefaultGlobalConfig.Instance.maxEnergy && timeToAddAllEnergy.Value != "")
        {
            ResetTimeToAddEnergy();
            ResetTimeToAddOneEnergy();
        }

        if (energyResource.Amount < DefaultGlobalConfig.Instance.maxEnergy)
        {
            //Debug.Log("save time to add energy");
            SaveTimeToAddOneEnergy();
        }

        EnergyDataSave.Instance.SaveData();
    }

    public bool IsEnoughEnergy()
    {
        return energyResource.Amount > 0;
    }

    public void RefillFullEnergy()
    {
        energyResource.Amount = DefaultGlobalConfig.Instance.maxEnergy;
        ResetTimeToAddEnergy();
        ResetTimeToAddOneEnergy();
        TimeManager.Instance.ClearScheduledEvents();
        SaveEnergyData();
    }

    public bool IsCanRefillEnergy()
    {
        return energyResource.Amount < DefaultGlobalConfig.Instance.maxEnergy;
    }
}