using System;
using Cysharp.Text;
using R3;
using TMPro;
using UniRx;
using UnityEngine;

public class TimeEnergy : MonoBehaviour
{
    public Reactive<string> timeToAddEnergy = new("");
    public Reactive<string> timeToEndInfinite = new("");

    public DateTime dTimeToAddEnergy;

    //public GameObject objShowTimeCooldown;
    public TextMeshProUGUI txtTimeCooldown;
    private bool addEvent;
    private bool isOnInfiniteEnergy;

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        timeToAddEnergy = EnergyManager.Instance.timeToAddOneEnergy;

        timeToEndInfinite = EnergyManager.Instance.timeToEndInfiniteEnergy;

        timeToAddEnergy.Subscribe(ChangeTimeAddOneEnergy).AddTo(this);

        timeToEndInfinite.Subscribe(ChangeTimeEndInfinite).AddTo(this);
    }

    private void ChangeTime()
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var timeChange = dTimeToAddEnergy.Subtract(currentTime);
        txtTimeCooldown.SetTextFormat(MyCache.strDefault, TimeUtil.TimeToString(timeChange.TotalSeconds));
    }

    private void ChangeTimeEndInfinite(string timeChange)
    {
        var activeTime = !timeChange.Equals("");
        if (activeTime)
            dTimeToAddEnergy = timeToEndInfinite.Value.ToDateTime();
        if (activeTime && !addEvent)
        {
           
            addEvent = true;
            isOnInfiniteEnergy = true;
            TimeManager.OnTimeChange += ChangeTime;
        }

        if (!activeTime && isOnInfiniteEnergy)
        {
            isOnInfiniteEnergy = false;
            txtTimeCooldown.SetTextFormat(MyCache.strDefault, "Full");
            TimeManager.OnTimeChange -= ChangeTime;
            addEvent = false;
        }
    }

    private void ChangeTimeAddOneEnergy(string timeChange)
    {
        var activeTime = !timeChange.Equals("");
        if (activeTime)
            dTimeToAddEnergy = timeToAddEnergy.Value.ToDateTime();
        
        if (activeTime && !addEvent)
        {
            addEvent = true;
            TimeManager.OnTimeChange += ChangeTime;
        }

        if (!activeTime)
        {
            txtTimeCooldown.SetTextFormat(MyCache.strDefault, "Full");
            TimeManager.OnTimeChange -= ChangeTime;
            addEvent = false;
        }
    }

    private void OnDestroy()
    {
        TimeManager.OnTimeChange -= ChangeTime;
    }
}