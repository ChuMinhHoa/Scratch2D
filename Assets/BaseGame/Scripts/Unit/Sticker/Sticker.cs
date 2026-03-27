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

    [field: SerializeField] public Reactive<bool> isDone { get; set; } = new(false);
    public Reactive<bool> isCallDone = new(false);
    
    [SerializeReference] public IRequireDoneSticker requireDoneSticker;
    public bool IsOnDoneState => stateMachine.CurrentState == StickerDoneState;

    private bool forceScratch = false;

    private void Start()
    {
        progress = stickerGraphic.scratchManager.Progress.reactiveCurrentProgress;
        progress.Subscribe(ChangeProgressCheck).AddTo(this);
        
        stateMachine.RequestTransition(StickerWaitState);
        stateMachine.Run();
    }

    private void ChangeProgressCheck(float progressChange)
    {
        if (isDone || stateMachine.CurrentState == StickerDoneState || isCallDone)
            return;
        if (!(progressChange >= progressDone)) return;
        
        isCallDone.Value = true;
        
        OnDoneProgress();
        stickerGraphic.FillAllScratch();
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
        forceScratch = false;
        isCallDone.Value = false;
    }

    protected void StickerMoveToTarget()
    {
        Level.Instance.RegisterStickerDone(this, stickerGraphic.currentRot, forceScratch);
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
        stickerGraphic.InitData(data.stickerID, rot);
        stateMachine.RequestTransition(StickerInitState);
    }

    public void EnableScratch(bool active)
    {
        stickerGraphic.EnableScratch(active);
    }

    public void ForceScratchDone()
    {
        forceScratch = true;
        stateMachine.RequestTransition(StickerDoneState);
        stickerGraphic.FillAllScratch();
    }
}
