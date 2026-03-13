using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using ScratchCardAsset;
using UnityEngine;

public class ScratchObject : MonoBehaviour
{
    public CardData cardData;
    public int layerIndex;
    public ScratchCardManager scratchCardManager;
    public Transform trsNeedRot;
    
    public async UniTask InitData(int layer, CardData data)
    {
        layerIndex = layer;
        cardData = data;
        var sprite = SpriteGlobalConfig.Instance.GetScratchCardSprite(data.cardType);
        scratchCardManager.ChangeSprite(sprite);
        scratchCardManager.gameObject.SetActive(true);
        await Wait();
    }

    private async UniTask Wait()
    {
        await UniTask.WaitForEndOfFrame();
        trsNeedRot.eulerAngles = cardData.rotation;
        var pos= cardData.position;
        pos.z = layerIndex;
        transform.position = pos;
    }

    public void EnableInput(bool isSameLayer)
    {
        scratchCardManager.EnableInput(isSameLayer);
    }

    public void SetActionCallBackChangeProgress(Action<float> callback)
    {
        scratchCardManager.SetActionCallBackChangeProgress(callback);
    }

    public void RemoveActionCallBackChangeProgress(Action<float> callback)
    {
        scratchCardManager.RemoveActionCallBackChangeProgress(callback);
    }

    public void AnimClose()
    {
        LMotion.Create(1f, 0f, 0.25f).WithOnComplete(() => { })
            .Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
    }
}
