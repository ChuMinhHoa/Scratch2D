using System;
using Cysharp.Threading.Tasks;
using R3;
using ScratchCardAsset;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;

public partial class Sticker : MonoBehaviour
{
    public StickerType stickerType;
    public StickerData stickerData;
    private readonly StateMachine stateMachine = new();
    
    public StickerGraphic stickerGraphic;
    
    public Reactive<float> progress = new(0);
    public float progressDone; 
    
    [field: SerializeField]
    public Reactive<bool> isDone { get; set; }
    
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
        var pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    protected void StickerMoveToTarget()
    {
        GamePlayManager.Instance.RegisterStickerDone(this, stickerGraphic.currentRot);
    }

    public void DisAbleIcon()
    {
        stickerGraphic.DisAbleIcon();
    }

    private bool CheckDoneRequire()
    {
        return requireDoneSticker.CheckDoneSticker();
    }

    public void InitData(StickerData data, Vector3 rot)
    {
        stickerData = data;
        stateMachine.RequestTransition(StickerInitState);
        stickerGraphic.InitData(data.stickerID, rot);
    }

    public void EnableScratch(bool sameLayer)
    {
        stickerGraphic.EnableScratch(sameLayer);
    }

    public void ScratchActive()
    {
        stickerGraphic.ScratchActive();
    }

    public void ForceScratchDone()
    {
        stateMachine.RequestTransition(StickerDoneState);
        stickerGraphic.FillAllScratch();
    }
}
