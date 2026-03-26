using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitMotion;
using ScratchCardAsset;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CardGraphic : MonoBehaviour
{
    public CardState cardState;
    public GameObject objDisable;


    public List<SpriteRenderer> sprAnim;
    public Color colorStartOpen;
    public Color colorEndOpen;
    public float timeOpen = 0.25f;
    public Vector3 offSetShadow;

    public Transform trsShadow;

    #region Lock State

    [ShowIf("@cardState == CardState.Lock")]
    public GameObject objLock;

    [ShowIf("@cardState == CardState.Lock")]
    public TextMeshPro txtCountUnlock;
    
    [ShowIf("@cardState == CardState.Lock")] 
    public LockAnimControl lockAnimControl;

    public void SetTextCount(int countRemaining)
    {
        Debug.Log("Cout remaining: " +countRemaining);
        txtCountUnlock.text = countRemaining.ToString();
    }

    public async UniTask UnLockCard()
    {
        lockAnimControl.PlayAnimUnlock();
        await UniTask.WaitForSeconds(.7f);
    }
    
    public async UniTask UnLockCardHammer()
    {
        lockAnimControl.PlayAnimUnlockByBooster();
        await UniTask.WaitForSeconds(.7f);
    }

    #endregion

    #region Freeze

    [ShowIf("@cardState == CardState.Freeze")]
    public GameObject objFreeze;
    
    [ShowIf("@cardState == CardState.Freeze")]
    public SpriteRenderer sprCardFreeze;

    [ShowIf("@cardState == CardState.Freeze")]
    public Sprite[] sprFreezes;

    [ShowIf("@cardState == CardState.Freeze")]
    public GameObject freezeParticle;
    
    public void SetSpriteFreeze(int index)
    {
        if (!sprCardFreeze.gameObject.activeSelf)
            sprCardFreeze.gameObject.SetActive(true);
        
        if (index - 1 < sprFreezes.Length)
        {
            sprCardFreeze.sprite = sprFreezes[index - 1];
        }
        freezeParticle.SetActive(true);
    }

    public void OnFreezeDone()
    {
        Debug.Log("Anim");
        objFreeze.SetActive(false);
        freezeParticle.SetActive(true);
    }

    #endregion Freeze State

    public void InitData(CardState cardStateChange)
    {
        cardState = cardStateChange;
        
        var isLock = cardState == CardState.Lock;
        objLock.SetActive(isLock);
        
        var isFreeze = cardState == CardState.Freeze;
        objFreeze.SetActive(isFreeze);
    }

    [Button]
    private async UniTask OnAnimOpen()
    {
        var currentPoint = transform.position;
        var targetPoint = currentPoint + offSetShadow;
        LMotion.Create(currentPoint, targetPoint, timeOpen).Bind(x => trsShadow.position = x).AddTo(this);
        await LMotion.Create(colorStartOpen, colorEndOpen, timeOpen).WithEase(Ease.InCubic).Bind(x =>
        {
            for (var i = 0; i < sprAnim.Count; i++)
            {
                sprAnim[i].color = x;
            }
        }).AddTo(this);
    }

    public async UniTask SetActiveObjLook(bool active)
    {
        trsShadow.gameObject.SetActive(!active);

        if (!active)
        {
            await OnAnimOpen();
        }

        objDisable.SetActive(active);
    }

    public async UniTask AnimCardDone(Action callBack = null)
    {
        callBack?.Invoke();
        await LMotion.Create(1f, 0f, 0.25f).WithOnComplete(() => { })
            .Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
    }

    [Button]
    public void ResetCard()
    {
        _ = SetActiveObjLook(true);
        for (var i = 0; i < sprAnim.Count; i++)
        {
            sprAnim[i].color = colorStartOpen;
        }
    }
}