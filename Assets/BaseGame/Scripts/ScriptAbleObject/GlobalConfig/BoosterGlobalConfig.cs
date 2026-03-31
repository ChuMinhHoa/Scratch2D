using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;

[CreateAssetMenu(fileName = "BoosterGlobalConfig", menuName = "GlobalConfigs/BoosterGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class BoosterGlobalConfig : GlobalConfig<BoosterGlobalConfig>
{
    public BoosterConfig[] boosterConfigs;
    
    public BoosterConfig GetBoosterConfig(BoosterType boosterType)
    {
        for (var i = 0; i < boosterConfigs.Length; i++)
        {
            if(boosterConfigs[i].boosterType == boosterType)
                return boosterConfigs[i];
        }

        return null;
    }

    public BigNumber GetPriceBoosterByReviveType(ReviveType slotData)
    {
        var boosterType = BoosterType.BoosterMagnet;
        switch (slotData)
        {
           
            case ReviveType.AddSlot:
                boosterType = BoosterType.BoosterAddSlot;
                break;
            case ReviveType.BoosterMagnet:
                boosterType = BoosterType.BoosterMagnet;
                break;
            case ReviveType.AddNote:
            default:
                return 0;
        }

        for (var i = 0; i < boosterConfigs.Length; i++)
        {
            if(boosterConfigs[i].boosterType == boosterType)
                return boosterConfigs[i].price;
        }

        return 0;
    }
}

[System.Serializable]
public class BoosterConfig
{
    [PreviewField] public Sprite icon;
    public BoosterType boosterType;
    public BigNumber price;
}