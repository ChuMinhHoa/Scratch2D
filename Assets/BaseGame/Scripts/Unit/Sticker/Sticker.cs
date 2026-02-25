using System;
using R3;
using ScratchCardAsset;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;

public partial class Sticker : MonoBehaviour
{
    private StateMachine stateMachine = new();
    
    public StickerGraphic stickerGraphic;
    
    public Reactive<float> progress = new(0);
    public float progressDone; 
    public ScratchCardManager scratchManager;
    public bool isDone;
    
    [SerializeReference] public IRequireDoneSticker requireDoneSticker;
    public bool IsOnDoneState => stateMachine.CurrentState == StickerDoneState;

    private void Start()
    {
        progress = scratchManager.Progress.reactiveCurrentProgress;
        progress.Subscribe(ChangeProgressCheck).AddTo(this);
        
        stateMachine.RequestTransition(StickerWaitState);
        stateMachine.Run();
    }

    public void InitSticker()
    {
        stateMachine.RequestTransition(StickerInitState);
    }

    private void ChangeProgressCheck(float progressChange)
    {
        if (isDone)
            return;
        if (progressChange >= progressDone)
        {
            isDone = true;
            OnDoneProgress();
            scratchManager.gameObject.SetActive(false);
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
        scratchManager.gameObject.SetActive(false);
        isDone = false;
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
}
