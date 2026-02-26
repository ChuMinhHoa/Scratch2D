using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using ScratchCardAsset;
using UniRx;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int layerIndex;
    public ScratchCardManager scratchCardManager;
    public Transform[] stickerPoints;

    public List<Sticker> stickers;
    public GameObject objLock;
    private Reactive<int> currentLayer = new(0);
    public int totalStickerScratchDone = 0;

    private void Start()
    {
        currentLayer = GamePlayManager.Instance.level.layerController.layerActive;
        currentLayer.Subscribe(OnChangeLayer).AddTo(this);
    }

    private void OnChangeLayer(int layer)
    {
        var isSameLayer = layerIndex == layer;
        objLock.SetActive(!isSameLayer);
    }

    public void SetActionCallbackChangeProgress(Action<float> callback)
    {
        scratchCardManager.SetActionCallBackChangeProgress(callback);
    }

    public void RemoveActionCallbackChangeProgress(Action<float> callback)
    {
        scratchCardManager.RemoveActionCallBackChangeProgress(callback);
    }

    public void LoadData(CardData cardData, int layer)
    {
        layerIndex = layer;
        for (var i = 0; i < cardData.stickers.Length; i++)
        {
            var sticker = PoolManager.Instance.SpawnSticker(stickerPoints[i]);
            sticker.transform.localPosition = Vector3.zero;
            sticker.InitData(cardData.stickers[i]);
            stickers.Add(sticker);
            sticker.isDone.Skip(1).Subscribe(ChangeStickerScratchDone).AddTo(this);
        }
    }

    private void ChangeStickerScratchDone(bool isDone)
    {
        totalStickerScratchDone++;
        if (totalStickerScratchDone >= stickers.Count)
        {
            Debug.Log("Anim card done");
            _ = AnimCardDone();
        }
    }

    private async UniTask AnimCardDone()
    {
        await UniTask.WaitForSeconds(2f);
        await LMotion.Create(1f, 0f, 0.25f).WithOnComplete(ResetCard).Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
    }

    private void ResetCard()
    {
        PoolManager.Instance.DespawnCard(this);
        transform.localScale = Vector3.one;
        for (var i = 0; i < stickers.Count; i++)
        {
            PoolManager.Instance.DespawnSticker(stickers[i]);
        }
        stickers.Clear();
    }
}