using System;
using System.Collections.Generic;
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

    public List<int> listID;

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
            TimeManager.Instance.RegisterEventTime(nextEnergyAddTime, AddOneEnergy, GetId());
            return;
        }

        AddOneEnergy(-1);
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

    private void SaveTimeToAddOneEnergy(bool addByAds = false)
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var defaultMinutesForEnergy = DefaultGlobalConfig.Instance.defaultMinutesForEnergy;
        var timeAddEnergyString = timeToAddOneEnergy.Value.Equals("") || addByAds
            ? currentTime.AddMinutes(defaultMinutesForEnergy).ToEnUsString()
            : timeToAddOneEnergy.Value.ToDateTime().AddMinutes(defaultMinutesForEnergy).ToEnUsString();
        timeToAddOneEnergy.Value = timeAddEnergyString;
        TimeManager.Instance.RegisterEventTime(timeAddEnergyString.ToDateTime(), AddOneEnergy, GetId());
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

    public void AddOneEnergy(int idCallBack, bool addByAds = false)
    {
        AddEnergy(1, addByAds);
        if (listID.Contains(idCallBack))
            listID.Remove(idCallBack);
    }

    private void AddEnergy(int amount, bool addByAds = false)
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
            SaveTimeToAddOneEnergy(addByAds);
        }

        EnergyDataSave.Instance.SaveData();
    }

    public bool IsEnoughEnergy()
    {
        return energyResource.Amount > 0;
    }

    public void RefillAddOnEnergy()
    {
        for (var i = listID.Count - 1; i >= 0; i--)
        {
            TimeManager.Instance.RemoveEvent(listID[i]);
            listID.RemoveAt(i);
        }
        AddOneEnergy(-1, true);
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

    private int GetId()
    {
        var idReturn = 0;
        while (listID.Contains(idReturn))
        {
            idReturn++;
        }

        listID.Add(idReturn);
        return idReturn;
    }
}