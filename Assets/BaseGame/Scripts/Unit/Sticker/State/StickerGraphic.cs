using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using ScratchCardAsset;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class StickerGraphic : MonoBehaviour
{
    public int stickerId;
    public StickerType stickerType;
    public SpriteRenderer sprIcon;
    public SpriteRenderer sprBg;
    public SpriteRenderer sprGlow;
    public UnitAnimation unitAnimation;
    public GameObject objGlow;
    public ScratchCardManager scratchManager;
    
    public SpriteRenderer objScratchFake;
    public Vector3 currentRot;
    [Button]
    public void InitData(int id, Vector3 rot)
    {
        stickerId = id;
        currentRot = rot;
    }

    public async UniTask InitStickerType(StickerType type)
    {
        stickerType = type;
        switch (stickerType)
        {
            case StickerType.Chain:
                InitStickerChain();
                break;
            case StickerType.Mark:
                InitStickerMark();
                break;
            case StickerType.Normal:
            default:
                InitStickerNormal();
                break;
        }
        
        sprIcon.gameObject.SetActive(true);
        scratchManager.ChangeSprite(sprBg.sprite);
        await ScratchActive();
    }

    [Button]
    public async UniTask OnDoneMode()
    {
        objGlow.SetActive(true);
        _ = unitAnimation.PlayScaleAnimation();
        var pos = transform.position;
        pos.z -= 0.02f;
        transform.position = pos;
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
        scratchManager.EnableInput(false);
        scratchManager.gameObject.SetActive(false);
    }

    public void FillAllScratch()
    {
        scratchManager.FillScratchCard();
    }

    private async UniTask ScratchActive()
    {
        scratchManager.gameObject.SetActive(true);
        objScratchFake.gameObject.SetActive(false);
        await UniTask.WaitForSeconds(0.1f);
        transform.eulerAngles = currentRot;
       
    }

    public void EnableScratch(bool active)
    {
        scratchManager.EnableInput(active);
    }

    public void SetParents(Transform currentParents)
    {
        transform.SetParent(currentParents);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
}
