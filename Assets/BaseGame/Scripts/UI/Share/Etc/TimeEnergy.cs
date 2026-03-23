using System;
using Cysharp.Text;
using R3;
using TMPro;
using UniRx;
using UnityEngine;

public class TimeEnergy : MonoBehaviour
{
    public Reactive<string> timeToAddEnergy = new("");
    public DateTime dTimeToAddEnergy;
    public GameObject objShowTimeCooldown;
    public TextMeshProUGUI txtTimeCooldown;

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        timeToAddEnergy = EnergyManager.Instance.timeToAddOneEnergy;
        timeToAddEnergy.Subscribe(ChangeTimeAddOneEnergy).AddTo(this);
        
    }

    private void ChangeTime()
    {
        var currentTime = TimeManager.Instance.GetCurrentTime();
      
        var timeChange = dTimeToAddEnergy.Subtract(currentTime);
        txtTimeCooldown.SetTextFormat(MyCache.strDefault, TimeUtil.TimeToString(timeChange.TotalSeconds));
    }

    private void ChangeTimeAddOneEnergy(string timeChange)
    {
        var activeTime = !timeChange.Equals("");
        dTimeToAddEnergy = timeToAddEnergy.Value.ToDateTime();
        objShowTimeCooldown.SetActive(activeTime);
        if (activeTime)
        {
            TimeManager.OnTimeChange += ChangeTime;
        }
        else
        {
            TimeManager.OnTimeChange -= ChangeTime;
        }
    }
}