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
        scratchManager.ChangeSprite(spriteBg);
    }

    private void InitStickerChain()
    {
    }

    private void InitStickerMark()
    {
        var spriteIcon = SpriteGlobalConfig.Instance.GetStickerIcon(stickerId);
        var spriteBg = SpriteGlobalConfig.Instance.GetStickerBg(stickerId);
        var spriteQuestMark = SpriteGlobalConfig.Instance.sprQuestMark;
        sprIcon.sprite = spriteIcon;
        sprBg.sprite = spriteBg;
        sprGlow.sprite = spriteIcon;
        scratchManager.ChangeSprite(spriteQuestMark);
    }
}
