using Sirenix.OdinInspector;
using UnityEngine;

public class InitStickerGraphic
{
}

public partial class StickerGraphic
{
    [ShowIf("@stickerType == StickerType.Mark")]
    public Sprite[] sprIconMark;
    private void InitStickerNormal()
    {
        var spriteIcon = SpriteGlobalConfig.Instance.GetStickerIcon(stickerId);
        var spriteBg = SpriteGlobalConfig.Instance.GetStickerBg(stickerId);
        
        sprIcon.sprite = spriteIcon;
        sprBg.sprite = spriteBg;
        sprGlow.sprite = spriteIcon;
        objScratchFake.sprite = spriteBg;
    }

    private void InitStickerChain()
    {
    }

    private void InitStickerMark()
    {
        sprIcon.sprite = sprIconMark[1];
        sprBg.sprite = sprIconMark[0];
        sprGlow.sprite = sprIconMark[2];
        objScratchFake.sprite = sprIconMark[0];
    }
}
