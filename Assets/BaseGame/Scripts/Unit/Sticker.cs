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
    
    private void Start()
    {
        progress = scratchManager.Progress.currentProgress;
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

    private void OnDoneProgress()
    {
        stateMachine.RequestTransition(StickerDoneState);
    }

    public void ResetSticker()
    {
        progress.Value = 0;
        scratchManager.gameObject.SetActive(false);
        isDone = false;
    }
}
