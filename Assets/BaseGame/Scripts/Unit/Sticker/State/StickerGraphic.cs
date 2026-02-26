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
    public UnitAnimation unitAnimation;
    public GameObject objGlow;
    public ScratchCardManager scratchManager;
    
    [Button]
    public void InitData(int id)
    {
        stickerId = id;
        var spriteIcon = SpriteGlobalConfig.Instance.GetStickerIcon(id);
        sprIcon.sprite = spriteIcon;
        sprBg.sprite = spriteIcon;
        sprGlow.sprite = spriteIcon;
        scratchManager.ChangeSprite(spriteIcon);
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
        objGlow.SetActive(false);
        scratchManager.Progress.ResetScratch();
        scratchManager.ResetScratchCard();
    }

    public void FillAllScratch()
    {
        scratchManager.FillScratchCard();
    }
}
