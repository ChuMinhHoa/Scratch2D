using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneMoveToObjHaveStickerState : IState
{
    public interface IHandler
    {
        UniTask OnEnterMoveToObjHaveStickerState();
        UniTask OnUpdateMoveToObjHaveStickerState();
        UniTask OnExitMoveToObjHaveStickerState();
    }

    private IHandler handler;

    public StickerDoneMoveToObjHaveStickerState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterMoveToObjHaveStickerState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateMoveToObjHaveStickerState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitMoveToObjHaveStickerState();
    }
}

public partial class StickerDone : StickerDoneMoveToObjHaveStickerState.IHandler
{
    private StickerDoneMoveToObjHaveStickerState StickerDoneMoveToObjHaveStickerStateCache { get; set; }

    public StickerDoneMoveToObjHaveStickerState StickerDoneMoveToObjHaveStickerState =>
        StickerDoneMoveToObjHaveStickerStateCache ??= new StickerDoneMoveToObjHaveStickerState(this);

    
    private Action actionCallBackOnMoveToNote;
    
    public async UniTask OnEnterMoveToObjHaveStickerState()
    {
        actionCallBackOnMoveToNote?.Invoke();
        var ct = this.GetCancellationTokenOnDestroy();
        
        var idRegister = UnitEventManager.Instance.RegisterEvent();
        CheckToAbleStickerAnimAgain();
        transform.SetParent(stickerPos.trsPos);
        var currentScale = transform.localScale;
        var currentEulerAngle = transform.eulerAngles;
        LMotion.Create(currentScale, Vector3.one, .25f).Bind(x => transform.localScale = x).AddTo(this);
        LMotion.Create(currentEulerAngle, stickerPos.trsPos.eulerAngles, .25f).Bind(x => transform.eulerAngles = x).AddTo(this);
        await unitAnim.PlayMoveAnimLocal(Vector3.zero);
        stickerDoneAnim.Play("StickerAdd");
        await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
        stickerGlow?.gameObject.SetActive(true);
        stickerPos.MoveDone();
        UnitEventManager.Instance.RemoveEventId(idRegister);
        await UniTask.WaitForSeconds(0.25f, cancellationToken: ct);
        Level.Instance.CheckLoseGame();
    }

    public UniTask OnUpdateMoveToObjHaveStickerState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitMoveToObjHaveStickerState()
    {
        return UniTask.CompletedTask;
    }
}