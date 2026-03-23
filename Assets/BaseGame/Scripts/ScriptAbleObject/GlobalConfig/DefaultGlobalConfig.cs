using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "DefaultGlobalConfig", menuName = "GlobalConfigs/DefaultGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class DefaultGlobalConfig : GlobalConfig<DefaultGlobalConfig>
{
    public int maxEnergy = 5;
    public float defaultMinutesForEnergy = 5; // 5 minutes for 1 energy
}