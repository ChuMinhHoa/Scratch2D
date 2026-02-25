using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "SpriteGlobalConfig", menuName = "GlobalConfigs/SpriteGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class SpriteGlobalConfig : GlobalConfig<SpriteGlobalConfig>
{
    public SpriteConfig<int>[] iconSpriteConfigs;
}

[System.Serializable]
public class SpriteConfig<TType>
{
    public TType tType;
    [PreviewField(100)] public Sprite sprite;
}