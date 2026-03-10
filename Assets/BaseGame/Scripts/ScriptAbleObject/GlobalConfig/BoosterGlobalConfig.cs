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
}

[System.Serializable]
public class BoosterConfig
{
    public Sprite icon;
    public BoosterType boosterType;
    public BigNumber price;
}