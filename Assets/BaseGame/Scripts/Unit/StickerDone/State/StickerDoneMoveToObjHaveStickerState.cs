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
        
        UnitEventManager.Instance.RegisterEvent(gameObject);
        CheckToAbleStickerAnimAgain();
        if (stickerPos == null)
        {
//            Debug.LogError("StickerDoneMoveToObjHaveStickerState: stickerPos is null");
            Level.Instance.fSpaceController.RegisterStickerDoneWait(this);
            return;
        }
        transform.SetParent(stickerPos.trsPos);
        var currentScale = transform.localScale;
        var currentEulerAngle = transform.eulerAngles;
        LMotion.Create(currentScale, Vector3.one, .25f).Bind(x => transform.localScale = x).AddTo(this);
        LMotion.Create(currentEulerAngle, stickerPos.trsPos.eulerAngles, .25f).Bind(x => transform.eulerAngles = x).AddTo(this);
        await MoveToPosLocal(Vector3.zero); // unitAnim.PlayMoveAnimLocal();
        stickerDoneAnim.Play("StickerAdd");
        SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_StickerDoneIn);
        await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
        stickerGlow?.gameObject.SetActive(true);
        stickerPos.MoveDone();
        UnitEventManager.Instance.RemoveEventId(gameObject);
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