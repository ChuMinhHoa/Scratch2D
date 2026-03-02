using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

public class CardGraphic : MonoBehaviour
{
    public GameObject objLock;
    public List<SpriteRenderer> sprAnim;
    public Color colorStartOpen;
    public Color colorEndOpen;
    public float timeOpen = 0.25f;

    private async UniTask OnAnimOpen()
    {
        await LMotion.Create(colorStartOpen, colorEndOpen, timeOpen).Bind(x =>
        {
            for (var i = 0; i < sprAnim.Count; i++)
            {
                sprAnim[i].color = x;
            }
        }).AddTo(this);
    }

    public async UniTask SetActiveObjLook(bool active)
    {
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

    public void ResetCard()
    {
        _ = SetActiveObjLook(true);
        for (var i = 0; i < sprAnim.Count; i++)
        {
            sprAnim[i].color = colorStartOpen;
        }
    }
}
