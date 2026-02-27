using System;
using R3;
using ScratchCardAsset;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;

public partial class Sticker : MonoBehaviour
{
    public StickerType stickerType;
    public StickerData stickerData;
    private StateMachine stateMachine = new();
    
    public StickerGraphic stickerGraphic;
    
    public Reactive<float> progress = new(0);
    public float progressDone; 
    
    public Reactive<bool> isDone;
    
    [SerializeReference] public IRequireDoneSticker requireDoneSticker;
    public bool IsOnDoneState => stateMachine.CurrentState == StickerDoneState;

    private void Start()
    {
        progress = stickerGraphic.scratchManager.Progress.reactiveCurrentProgress;
        progress.Subscribe(ChangeProgressCheck).AddTo(this);
        
        stateMachine.RequestTransition(StickerWaitState);
        stateMachine.Run();
    }

    private void ChangeProgressCheck(float progressChange)
    {
        if (isDone)
            return;
        if (progressChange >= progressDone)
        {
            isDone.Value = true;
            OnDoneProgress();
            stickerGraphic.FillAllScratch();
        }
    }

    protected internal void OnDoneProgress()
    {
        if (!CheckDoneRequire())
            return;
        stateMachine.RequestTransition(StickerDoneState);
    }

    public void ResetSticker()
    {
        progress.Value = 0;
        isDone.Value = false;
        stickerGraphic.ResetGraphic();
    }

    protected void StickerMoveToTarget()
    {
        GamePlayManager.Instance.RegisterStickerDone(this);
    }

    public void DisAbleIcon()
    {
        stickerGraphic.DisAbleIcon();
    }

    private bool CheckDoneRequire()
    {
        return requireDoneSticker.CheckDoneSticker();
    }

    public void InitData(StickerData data)
    {
        stickerData = data;
        stateMachine.RequestTransition(StickerInitState);
        stickerGraphic.InitData(data.stickerID);
    }

    public void EnableScratch(bool sameLayer)
    {
        stickerGraphic.EnableScratch(sameLayer);
    }

    public void ScratchActive()
    {
        stickerGraphic.ScratchActive();
    }
}
