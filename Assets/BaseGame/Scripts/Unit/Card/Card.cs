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
    public Transform[] stickerPoints;

    public List<Sticker> stickers;
    public int countSticker;
 
    private Reactive<int> currentLayer = new(0);
    public AnimationCurve curveFirstSpawn;
    public GameObject objFakeScratch;

    private CompositeDisposable stickerSubscriptions = new CompositeDisposable();
    private bool isSameLayer;
    private Reactive<bool> isOnPlaying = new();
    
    public StateMachine stateMachine = new();
    public CardGraphic cardGraphic;

    public ScratchObject scratchObject;

    private void Start()
    {
        currentLayer = GamePlayManager.Instance.level.layerController.layerActive;
        currentLayer.Skip(1).Subscribe(OnChangeLayer).AddTo(this);
        
        stateMachine.RequestTransition(CardWaitState);
        stateMachine.Run();
        isOnPlaying = GamePlayManager.Instance.onPlaying;
        isOnPlaying.Skip(1).Subscribe(ChangeGameState).AddTo(this);
    }

    private void ChangeGameState(bool playing)
    {
        isSameLayer = layerIndex == currentLayer.Value && playing;
        EnableInput();
    }

    private void OnChangeLayer(int layer)
    {
        if (!stateMachine.IsCurrentState(CardWaitState)) return;
        if (IsDone()) return;
        if(GamePlayManager.Instance.level.IsHaveStickerWait()) return;
        isSameLayer = layerIndex == layer && isOnPlaying.Value;
        _ = WaitToEnableInput();
    }
    
    private async UniTask WaitToEnableInput()
    {
        await UniTask.WaitForSeconds(1f);
        await cardGraphic.SetActiveObjLook(!isSameLayer);
        EnableInput();
    }

    private void EnableInput()
    {
        for (var i = 0; i < stickers.Count; i++)
        {
            stickers[i].EnableScratch(isSameLayer);
        }
        scratchObject.EnableInput(isSameLayer);
    }

    public void SetActionCallbackChangeProgress(Action<float> callback)
    {
        scratchObject.SetActionCallBackChangeProgress(callback);
    }

    public void RemoveActionCallbackChangeProgress(Action<float> callback)
    {
        scratchObject.RemoveActionCallBackChangeProgress(callback);
    }

    private void ChangeStickerScratchDone(bool isDone)
    {
        if (!isDone) return;
        countSticker--;
        if (countSticker == 0)
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
        Destroy(scratchObject.gameObject);
        
        stickerSubscriptions.Clear();
        PoolManager.Instance.DespawnCard(this);
        GamePlayManager.Instance.RemoveCurrentCard(this);
        cardGraphic.ResetCard();
        objFakeScratch.SetActive(true);
    }

    [Button]
    public void AnimFirstSpawn(int index)
    {
        LMotion.Create(0f, 1f, 0.25f).WithOnComplete(() =>
            {
                isSameLayer = layerIndex == 0;
                _ = cardGraphic.SetActiveObjLook(!isSameLayer);
               
                for (var i = 0; i < stickers.Count; i++)
                {
                    //stickers[i].ScratchActive();
                    stickers[i].EnableScratch(isSameLayer);
                }

                _ = SetScratchObject();
                
            }).WithDelay(0.15f * index).WithEase(curveFirstSpawn).Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
    }

    private async UniTask SetScratchObject()
    {
        scratchObject.EnableInput(isSameLayer);
        await scratchObject.InitData(layerIndex, data);
        objFakeScratch.SetActive(false);
    }

    public bool CheckIsSameLayer()
    {
        return layerIndex == currentLayer.Value;
    }

    public bool IsDone() => countSticker == 0;

    public int IsHaveSticker(int stickerId, int countRemain)
    {
        var count = 0;
        for (var i = 0; i < stickers.Count; i++)
        {
            if (stickers[i].stickerData.stickerID == stickerId && !stickers[i].isDone)
            {
                stickers[i].ForceScratchDone();
                count++;
                if (count == countRemain)
                    return 0;
            }
        }
        return countRemain - count;
    }
    
    public bool IsHaveSticker(int stickerId)
    {
        for (var i = 0; i < stickers.Count; i++)
        {
            
            Debug.Log($"note id {stickerId} {stickers[i].stickerData.stickerID}");
            if (stickers[i].stickerData.stickerID == stickerId && !stickers[i].isDone)
            {
                return true;
            }
        }

        return false;
    }
}