using NUnit.Framework;
using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;

[CreateAssetMenu(fileName = "ComboGlobalConfig", menuName = "GlobalConfigs/ComboGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ComboGlobalConfig : GlobalConfig<ComboGlobalConfig>
{
    public ComboConfig[] comboConfigs;

     public ComboConfig GetCoinRewardByComboCount(int comboCount)
     {
         for (var i = 0; i < comboConfigs.Length; i++)
         {
             if (comboConfigs[i].comboCount <= comboCount)
             {
                 return comboConfigs[i];
             }
         }

         return null;
     }
}

[System.Serializable]
public class ComboConfig
{
    public int comboCount;
    public BigNumber coinReward;
}