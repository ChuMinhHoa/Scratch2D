using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using ScratchCardAsset;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;
using CompositeDisposable = R3.CompositeDisposable;

public partial class Card : MonoBehaviour
{
    public int layerIndex;
    public ScratchCardManager scratchCardManager;
    public Transform[] stickerPoints;

    public List<Sticker> stickers;
    public GameObject objLock;
    private Reactive<int> currentLayer = new(0);
    public int totalStickerScratchDone = 0;
    public AnimationCurve curveFirstSpawn;
    public GameObject objFakeScratch;

    private CompositeDisposable stickerSubscriptions = new CompositeDisposable();
    private bool isSameLayer;
    
    public StateMachine stateMachine = new();

    private void Start()
    {
        currentLayer = GamePlayManager.Instance.level.layerController.layerActive;
        currentLayer.Skip(1).Subscribe(OnChangeLayer).AddTo(this);
        
        stateMachine.RequestTransition(CardWaitState);
        stateMachine.Run();
    }

    private void OnChangeLayer(int layer)
    {
        if (IsDone()) return;
        isSameLayer = layerIndex == layer;
        _ = WaitToEnableInput();
    }
    
    private async UniTask WaitToEnableInput()
    {
        await UniTask.WaitForSeconds(1f);
        objLock.SetActive(!isSameLayer);
        for (var i = 0; i < stickers.Count; i++)
        {
            stickers[i].EnableScratch(isSameLayer);
        }
        scratchCardManager.EnableInput(isSameLayer);
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
        var pos = transform.position;
        pos.z = layer;
        transform.position = pos;
        for (var i = 0; i < cardData.stickers.Length; i++)
        {
            var sticker = PoolManager.Instance.SpawnSticker(stickerPoints[i]);
            sticker.transform.localPosition = Vector3.zero;
            sticker.InitData(cardData.stickers[i]);
            stickers.Add(sticker);
            stickerSubscriptions.Add(sticker.isDone.Skip(1).Subscribe(ChangeStickerScratchDone));
        }
       
    }

    private void ChangeStickerScratchDone(bool isDone)
    {
        if (!isDone) return;
        totalStickerScratchDone++;
        if (totalStickerScratchDone >= stickers.Count)
        {
            _ = AnimCardDone();
        }
    }

    private async UniTask AnimCardDone()
    {
        Debug.Log("anim done");
        await UniTask.WaitForSeconds(2f);
        scratchCardManager.EnableInput(false);
        await LMotion.Create(1f, 0f, 0.25f).WithOnComplete(() => {  })
            .Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
        ResetCard();
        GamePlayManager.Instance.NextLayer();
    }

    private void ResetCard()
    {
        transform.localScale = Vector3.one;
        for (var i = 0; i < stickers.Count; i++)
        {
            stickers[i].ResetSticker();
            PoolManager.Instance.DespawnSticker(stickers[i]);
        }

        stickers.Clear();
        scratchCardManager.Progress.ResetScratch();
        totalStickerScratchDone = 0;
        stickerSubscriptions.Clear();
        PoolManager.Instance.DespawnCard(this);
        GamePlayManager.Instance.RemoveCurrentCard(this);
    }

    public void AnimFirstSpawn(int index)
    {
        LMotion.Create(0f, 1f, 0.25f).WithOnComplete(() =>
            {
                isSameLayer = layerIndex == 0;
                objLock.SetActive(!isSameLayer);
                scratchCardManager.gameObject.SetActive(true);
                objFakeScratch.gameObject.SetActive(false);
                for (var i = 0; i < stickers.Count; i++)
                {
                    stickers[i].ScratchActive();
                    stickers[i].EnableScratch(isSameLayer);
                }
                scratchCardManager.EnableInput(isSameLayer);
            }).WithDelay(0.15f * index).WithEase(curveFirstSpawn).Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
    }

    public bool CheckIsSameLayer()
    {
        return layerIndex == currentLayer.Value;
    }
    
    public bool IsDone() => totalStickerScratchDone >= stickers.Count;
}