using System;
using TW.Utility.DesignPattern;
using UnityEngine;

public class PlayerInfoManager : Singleton<PlayerInfoManager>
{
    public Reactive<int> playerLevel = new (0);
    public bool loadDone = false;

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        playerLevel = PlayerInfoDataSave.Instance.playerLevel;
        loadDone = true;
    }
}
