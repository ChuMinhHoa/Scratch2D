using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneWaitOnCartState : IState
{
    public interface IHandler
    {
        UniTask OnEnterWaitOnCartState();
        UniTask OnUpdateWaitOnCartState();
        UniTask OnExitWaitOnCartState();
    }

    private IHandler handler;

    public StickerDoneWaitOnCartState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterWaitOnCartState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateWaitOnCartState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitWaitOnCartState();
    }
}

public partial class StickerDone : StickerDoneWaitOnCartState.IHandler
{
    private StickerDoneWaitOnCartState StickerDoneWaitOnCartStateCache { get; set; }

    public StickerDoneWaitOnCartState StickerDoneWaitOnCartState =>
        StickerDoneWaitOnCartStateCache ??= new StickerDoneWaitOnCartState(this);

    public async UniTask OnEnterWaitOnCartState()
    {
        if (stickerPos != null)
        {
            stickerPos.ResetPos();
            stickerPos = null;
            var targetPos = Level.Instance.fSpaceController.cartBooster.GetPosStickerDone();
            //var currentScale = transform.localScale;
            //LMotion.Create(currentScale, Vector3.zero, unitAnimMoveToCart.timeMove).Bind(x=> transform.localScale = x).AddTo(this);
            await MoveToCart(targetPos);
            transform.localScale = Vector3.zero;
            await UniTask.WaitForSeconds(0.7f);
            Level.Instance.CheckStickerDone();
        }
    }

    public UniTask OnUpdateWaitOnCartState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitWaitOnCartState()
    {
        return UniTask.CompletedTask;
    }
}