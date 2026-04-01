using System;
using System.Collections.Generic;
using LitMotion;
using UnityEngine;

public class CartObjBooster : MonoBehaviour
{
    [SerializeField] private GameObject objProof;
    [SerializeField] private Transform cartTrs;
    [SerializeField] private bool isActive;
    private IBooster booster;
    public List<StickerDone> stickerDone = new();
    
    public void OnUseCardBooster(IBooster iBooster)
    {
        booster = iBooster;
        isActive = true;
        var currentScale = cartTrs.localScale;
        objProof.gameObject.SetActive(true);
        LMotion.Create(currentScale, Vector3.one, 0.15f).Bind(x => cartTrs.localScale = x).AddTo(this);
        Level.Instance.fSpaceController.UseBoosterCart();
    }

    public void AddStickerDone(StickerDone stickerD)
    {
        var currentPos = stickerD.transform.position;
        stickerD.stateMachine.RequestTransition(stickerD.StickerDoneWaitOnCartState);
        //LMotion.Create(currentPos, transform.position, 0.15f).Bind(x => stickerD.transform.position = x).AddTo(this);
        stickerDone.Add(stickerD);
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
}
