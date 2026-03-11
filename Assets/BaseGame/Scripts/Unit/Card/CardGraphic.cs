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
    public GameObject objLock;
    public List<SpriteRenderer> sprAnim;
    public Color colorStartOpen;
    public Color colorEndOpen;
    public float timeOpen = 0.25f;
    public Vector3 offSetShadow;

    public Transform trsShadow;
    
    //public ScratchCardManager scratchCardManager;

    [Button]
    public void InitData()
    {
        //trsShadow.position = transform.position + offSetShadow;
    }

    [Button]
    private async UniTask OnAnimOpen()
    {
        var currentPoint = transform.position;
        var targetPoint = currentPoint + offSetShadow;
        LMotion.Create(currentPoint, targetPoint, timeOpen).Bind(x => trsShadow.position = x);
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
        objLock.SetActive(active);
    }

    public async UniTask AnimCardDone(Action callBack = null)
    {
        await UniTask.WaitForSeconds(1f);
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
