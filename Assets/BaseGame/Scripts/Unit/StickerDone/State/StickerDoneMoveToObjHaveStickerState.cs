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

    public async UniTask OnEnterMoveToObjHaveStickerState()
    {
        var idRegister = UnitEventManager.Instance.RegisterEvent();
        CheckToAbleStickerAnimAgain();
        var currentScale = transform.localScale;
        var currentEulerAngle = transform.eulerAngles;
        LMotion.Create(currentScale, stickerPos.trsPos.localScale, .25f).Bind(x => transform.localScale = x);
        LMotion.Create(currentEulerAngle, stickerPos.trsPos.eulerAngles, .25f).Bind(x => transform.eulerAngles = x);
        await unitAnim.PlayMoveAnim(stickerPos.trsPos.position);
        stickerDoneAnim.Play("StickerAdd");
        await UniTask.WaitForSeconds(0.5f);
        stickerGlow.gameObject.SetActive(true);
        transform.SetParent(stickerPos.trsPos);
        stickerPos.MoveDone();
        UnitEventManager.Instance.RemoveEventId(idRegister);
        await UniTask.WaitForSeconds(0.25f);
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