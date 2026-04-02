using System;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using LitMotion;
using TMPro;
using UnityEngine;

public class CartObjBooster : MonoBehaviour
{
    [SerializeField] private GameObject objProof;
    [SerializeField] private Transform cartTrs;
    [SerializeField] private Transform pointStickerDone;
    [SerializeField] private bool isActive;
    private IBooster booster;
    public List<StickerDone> stickerDone = new();
    [SerializeField] private CartBoosterGraphic cartGraphic;
    public TextMeshPro txtCountStickerDone;
    
    public async UniTask OnUseCardBooster(IBooster iBooster)
    {
        booster = iBooster;
        objProof.gameObject.SetActive(true);
        await WaitForAnimSpawn();
    }
    
    private async UniTask WaitForAnimSpawn()
    {
        if (!isActive)
        {
            isActive = true;
            cartGraphic.PlayAnimSpawn();
            SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_BoosterAddSlot);
            await UniTask.WaitForSeconds(0.25f);
            cartGraphic.PlayAnimOpen();
            Level.Instance.fSpaceController.SetPositionSpaceSticker();
            await UniTask.WaitForSeconds(0.5f);
        }
        else
        {
            Level.Instance.fSpaceController.SetPositionSpaceSticker();
        }

        await Level.Instance.fSpaceController.UseBoosterCart();
    }

    public async UniTask AddStickerDone(StickerDone stickerD, int index)
    {
        stickerDone.Add(stickerD);
        await UniTask.WaitForSeconds(0.1f * index);
        stickerD.stateMachine.RequestTransition(stickerD.StickerDoneWaitOnCartState);
            cartGraphic.PlayAnimCollect();
        txtCountStickerDone.SetTextFormat(MyCache.strDefault, stickerDone.Count);
    }

    public void PlayAnimClose()
    {
    }

    public void RemoveStickerDoneFromCart(StickerDone sticker)
    {
        if (!stickerDone.Contains(sticker)) return;
        cartGraphic.PlayAnimCollect();
        SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_StickerDoneFSpace);
        stickerDone.Remove(sticker);
        txtCountStickerDone.SetTextFormat(MyCache.strDefault, stickerDone.Count);
    }

    public bool IsCanUseBoosterCart()
    {
        return Level.Instance.fSpaceController.IsCanUseBoosterCart();
    }

    public void ResetCart()
    {
        isActive = false;
        //cartTrs.localScale = Vector3.zero;
        cartGraphic.PlayAnimIdle();
        //Level.Instance.fSpaceController.SetPositionSpaceSticker();
    }

    public void CheckStickerDone()
    {
        for (var i = stickerDone.Count - 1; i >= 0 ; i--)
        {
            StickerDoneManager.Instance.AddStickerDone(stickerDone[i]);
        }
    }
    
    public Vector3 GetPosStickerDone()
    {
        return pointStickerDone.position;
    }

    public bool IsActiveBooster() => isActive;

    private MotionHandle moveHandle;

    public void MoveCartObj(Vector3 pos)
    {
        if (moveHandle.IsActive())
            moveHandle.TryCancel();
        var currentPos = transform.localPosition;
        moveHandle = LMotion.Create(currentPos, pos, 0.15f).Bind(x => transform.localPosition = x);
        for (var i = 0; i < stickerDone.Count; i++)
        {
            var newPos = transform.parent.TransformPoint(pos);
            stickerDone[i].MoveToFreeSpaceOnUseBooster(newPos);
        }
    }
}
