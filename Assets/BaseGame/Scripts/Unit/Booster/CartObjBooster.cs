using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
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
    
    public void OnUseCardBooster(IBooster iBooster)
    {
        booster = iBooster;
        isActive = true;
        var currentScale = cartTrs.localScale;
        objProof.gameObject.SetActive(true);
        //LMotion.Create(currentScale, Vector3.one, 0.15f).Bind(x => cartTrs.localScale = x).AddTo(this);
        _ = WaitForAnimSpawn();
    }
    
    private async UniTask WaitForAnimSpawn()
    {
        cartGraphic.PlayAnimSpawn();
        await UniTask.WaitForSeconds(0.15f);
        cartGraphic.PlayAnimOpen();
        await UniTask.WaitForSeconds(0.5f);
        Level.Instance.fSpaceController.UseBoosterCart();
    }

    public async UniTask AddStickerDone(StickerDone stickerD, int index)
    {
        stickerDone.Add(stickerD);
        await UniTask.WaitForSeconds(0.1f * index);
        stickerD.stateMachine.RequestTransition(stickerD.StickerDoneWaitOnCartState);
        cartGraphic.PlayAnimCollect();
    }
    
    public void RemoveStickerDone(StickerDone stickerD)
    {
        stickerDone.Remove(stickerD);
    }

    public bool IsCanUseBoosterCart()
    {
        return !isActive;
    }

    public void ResetCart()
    {
        isActive = false;
        cartTrs.localScale = Vector3.zero;
    }

    public void CheckStickerDone()
    {
        for (var i = stickerDone.Count - 1; i >=0 ; i--)
        {
            StickerDoneManager.Instance.AddStickerDone(stickerDone[i]);
        }
    }

    public void RemoveStickerDoneFromCart(StickerDone sticker)
    {
        stickerDone.Remove(sticker);
    }

    public Vector3 GetPosStickerDone()
    {
        return pointStickerDone.position;
    }
}
