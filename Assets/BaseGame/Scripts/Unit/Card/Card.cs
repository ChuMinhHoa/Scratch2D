using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;
using CompositeDisposable = R3.CompositeDisposable;

public partial class Card : MonoBehaviour
{
    public CardType cardType;
    public int layerIndex;
    public Transform[] stickerPoints;

    public List<Sticker> stickers;
    public int countSticker;

    public AnimationCurve curveFirstSpawn;
    //public GameObject objFakeScratch;

    private CompositeDisposable stickerSubscriptions = new CompositeDisposable();
    private Reactive<bool> isOnPlaying = new();

    public StateMachine stateMachine = new();
    public CardGraphic cardGraphic;

    public ScratchObject scratchObject { get; set; }
    public FrontChecker2D frontChecker2D;

    public bool isShowed = false;

    private void Start()
    {
        stateMachine.RequestTransition(CardWaitState);
        stateMachine.Run();
        isOnPlaying = GamePlayManager.Instance.onPlaying;
        isOnPlaying.Skip(1).Subscribe(ChangeGameState).AddTo(this);
        GlobalEventManager.OnHaveCardDone += CheckToShow;
    }

    private void OnDestroy()
    {
        GlobalEventManager.OnHaveCardDone -= CheckToShow;
    }

    [Button]
    private void CheckToShow()
    {
        if (!stateMachine.IsCurrentState(CardWaitState)) return;
        
        if (IsDone()) return;
        
        if(Level.Instance.IsHaveStickerWait()) return;

        if (isShowed) return;
        
        var result = frontChecker2D.IsAnythingInFront();
        if (!result)
        {
            isShowed = true;
            OnShowMode();
        }
    }
    
    private void OnShowMode()
    {
        _ = cardGraphic.SetActiveObjLook(false);
        EnableInput(true);
        for (var i = 0; i < stickers.Count; i++)
        {
            stickers[i].EnableScratch(true);
        }
        //objFakeScratch.SetActive(false);
    }

    private void ChangeGameState(bool playing)
    {
        //EnableInput(playing);
    }
    
    private void EnableInput(bool isEnable)
    {
        for (var i = 0; i < stickers.Count; i++)
        {
            stickers[i].EnableScratch(isEnable);
        }
        scratchObject.EnableInput(isEnable);
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

    public void ResetCard()
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
        //objFakeScratch.SetActive(true);
        isShowed = false;
    }

    [Button]
    public void AnimFirstSpawn(int index)
    {
        LMotion.Create(0f, 1f, 0.25f).WithDelay(0.15f * index).WithEase(curveFirstSpawn).Bind(x =>
            {
                transform.localScale = Vector3.one * x;
                scratchObject.transform.localScale = Vector3.one * x;
            })
            .AddTo(this);
    }

    private bool IsDone() => countSticker == 0;

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

    public void SettingForFirstAnim()
    {
        transform.localScale = Vector3.zero;
        scratchObject.transform.localScale = Vector3.zero;
    }
}