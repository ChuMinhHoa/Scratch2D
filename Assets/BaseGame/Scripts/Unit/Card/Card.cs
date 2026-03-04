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
using UnityEngine.Serialization;
using CompositeDisposable = R3.CompositeDisposable;

public partial class Card : MonoBehaviour
{
    public CardType cardType;
    public int layerIndex;
    public ScratchCardManager scratchCardManager;
    public Transform[] stickerPoints;

    public List<Sticker> stickers;
 
    private Reactive<int> currentLayer = new(0);
    public AnimationCurve curveFirstSpawn;
    public GameObject objFakeScratch;

    private CompositeDisposable stickerSubscriptions = new CompositeDisposable();
    private bool isSameLayer;
    
    public StateMachine stateMachine = new();
    public CardGraphic cardGraphic;

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
        await cardGraphic.SetActiveObjLook(!isSameLayer);
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

    private void ChangeStickerScratchDone(bool isDone)
    {
        if (!isDone) return;
        for (var i = stickers.Count - 1; i >=0; i--)
        {
            if(stickers[i].isDone) stickers.Remove(stickers[i]);
        }
        if (stickers.Count == 0)
        {
            stateMachine.RequestTransition(CardDoneState);
        }
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
        stickerSubscriptions.Clear();
        PoolManager.Instance.DespawnCard(this);
        GamePlayManager.Instance.RemoveCurrentCard(this);
        cardGraphic.ResetCard();
    }

    public void AnimFirstSpawn(int index)
    {
        LMotion.Create(0f, 1f, 0.25f).WithOnComplete(() =>
            {
                isSameLayer = layerIndex == 0;
                _ = cardGraphic.SetActiveObjLook(!isSameLayer);
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

    public bool IsDone() => stickers.Count == 0;

    public bool IsHaveSticker(int stickerId, out Sticker sticker)
    {
        for (var i = 0; i < stickers.Count; i++)
        {
            if (stickers[i].stickerData.stickerID == stickerId)
            {
                sticker = stickers[i];
                return true;
            }
        }
        sticker = null;
        return false;
    }
}