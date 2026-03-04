using Cysharp.Threading.Tasks;
using LitMotion;
using ScratchCardAsset;
using Sirenix.OdinInspector;
using UnityEngine;

public class StickerGraphic : MonoBehaviour
{
    public int stickerId;
    public SpriteRenderer sprIcon;
    public SpriteRenderer sprBg;
    public SpriteRenderer sprGlow;
    public SpriteRenderer sprShadow;
    public UnitAnimation unitAnimation;
    public GameObject objGlow;
    public ScratchCardManager scratchManager;
    
    public SpriteRenderer objScratchFake;
    
    [Button]
    public void InitData(int id)
    {
        stickerId = id;
        
        var spriteIcon = SpriteGlobalConfig.Instance.GetStickerIcon(id);
        var spriteBg = SpriteGlobalConfig.Instance.GetStickerBg(id);
        var spriteShadow = SpriteGlobalConfig.Instance.GetStickerShaDow(id);
        
        sprIcon.sprite = spriteIcon;
        sprBg.sprite = spriteBg;
        sprGlow.sprite = spriteIcon;
        objScratchFake.sprite = spriteBg;
        sprShadow.sprite = spriteShadow;
    }

    [Button]
    public async UniTask OnDoneMode()
    {
        objGlow.SetActive(true);
        _ = unitAnimation.PlayScaleAnimation();
        await LMotion.Create(0f, 1f, 0.5f).WithOnComplete(() =>
        {
            objGlow.SetActive(false);
        }).RunWithoutBinding().AddTo(this);
    }

    public void DisAbleIcon()
    {
        sprIcon.gameObject.SetActive(false);
    }
    
    [Button]
    public void ResetGraphic()
    {
        sprIcon.gameObject.SetActive(true); 
        objScratchFake.gameObject.SetActive(true);
        objGlow.SetActive(false);
        scratchManager.ResetScratchCard();
        scratchManager.Progress.ResetScratch();
        scratchManager.gameObject.SetActive(false);
    }

    public void FillAllScratch()
    {
        scratchManager.FillScratchCard();
    }

    public void ScratchActive()
    {
        scratchManager.gameObject.SetActive(true);
        var spriteScratch = SpriteGlobalConfig.Instance.GetStickerBg(stickerId);
        scratchManager.ChangeSprite(spriteScratch);
        objScratchFake.gameObject.SetActive(false);
    }

    public void EnableScratch(bool sameLayer)
    {
        scratchManager.EnableInput(sameLayer);
    }
}
