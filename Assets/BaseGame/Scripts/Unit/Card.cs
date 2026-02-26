using System;
using System.Collections.Generic;
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
        }
    }
}