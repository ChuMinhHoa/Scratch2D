using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpriteGlobalConfig", menuName = "GlobalConfigs/SpriteGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class SpriteGlobalConfig : GlobalConfig<SpriteGlobalConfig>
{
    public SpriteConfig<int>[] iconSpriteConfigs;

    public Sprite GetStickerIcon(int id)
    {
        for (var i = 0; i < iconSpriteConfigs.Length; i++)
        {
            if(iconSpriteConfigs[i].tType == id)
                return iconSpriteConfigs[i].sprite;
        }

        return null;
    }

    public int GetRandomStickerId()
    {
        var randomIndex = Random.Range(0, iconSpriteConfigs.Length);
        return iconSpriteConfigs[randomIndex].tType;
    }

    public int GetSpriteID(Sprite sprite)
    {
        for (var i = 0; i < iconSpriteConfigs.Length; i++)
        {
            if(iconSpriteConfigs[i].sprite == sprite)
                return iconSpriteConfigs[i].tType;
        }

        return -1;
    }

    public SpriteConfig<int>[] iconObjHaveStickerConfigs;
    public Sprite GetIconObjectHaveSticker(int dataObjID)
    {
        for (var i = 0; i < iconObjHaveStickerConfigs.Length; i++)
        {
            if(iconObjHaveStickerConfigs[i].tType == dataObjID)
                return iconObjHaveStickerConfigs[i].sprite;
        }

        return null;
    }

    public SpriteConfig<int>[] iconStickerBgConfigs;
    
    public Sprite GetStickerBg(int id)
    {
        for (var i = 0; i < iconStickerBgConfigs.Length; i++)
        {
            if(iconStickerBgConfigs[i].tType == id)
                return iconStickerBgConfigs[i].sprite;
        }

        return null;
    }

    
    public SpriteConfig<int>[] iconStickerShadowConfigs;
    public Sprite GetStickerShaDow(int id)
    {
        for (var i = 0; i < iconStickerShadowConfigs.Length; i++)
        {
            if(iconStickerShadowConfigs[i].tType == id)
                return iconStickerShadowConfigs[i].sprite;
        }

        return null;
    }
}

[System.Serializable]
public class SpriteConfig<TType>
{
    public TType tType;
    [PreviewField(100)] public Sprite sprite;
}