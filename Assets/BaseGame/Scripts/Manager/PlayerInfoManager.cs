using System;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInfoManager : Singleton<PlayerInfoManager>
{
    public Reactive<int> playerLevel = new (0);
    public bool loadDone = false;
    public Reactive<int> levelChange = new (-1);
    public int adsRewardCount = 0;
    public int adsInterCount = 0;

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        playerLevel = PlayerInfoDataSave.Instance.playerLevel;
        levelChange = PlayerInfoDataSave.Instance.levelChange;
        adsRewardCount = PlayerInfoDataSave.Instance.adsRewardCount;
        adsInterCount = PlayerInfoDataSave.Instance.adsInterCount;
        loadDone = true;
      
    }

    public void AddAdsReward()
    {
        adsRewardCount++;
        PlayerInfoDataSave.Instance.SaveData();
    }

    public void AddAdsInter()
    {
        adsInterCount++;
        PlayerInfoDataSave.Instance.SaveData();
    }
}
