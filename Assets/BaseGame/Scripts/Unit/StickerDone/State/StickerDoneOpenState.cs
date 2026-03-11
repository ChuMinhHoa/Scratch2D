using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneOpenState : IState
{
    public interface IHandler
    {
        UniTask OnEnterOpenState();
        UniTask OnUpdateOpenState();
        UniTask OnExitOpenState();
    }

    private IHandler handler;

    public StickerDoneOpenState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterOpenState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateOpenState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitOpenState();
    }
}

public partial class StickerDone : StickerDoneOpenState.IHandler
{
    private StickerDoneOpenState StickerDoneOpenStateCache { get; set; }
    public StickerDoneOpenState StickerDoneOpenState => StickerDoneOpenStateCache ??= new StickerDoneOpenState(this);

    public async UniTask OnEnterOpenState()
    {
        CheckToAbleStickerAnimAgain();
        stickerDoneAnim.Play("StickerRemove");
        await UniTask.WaitForSeconds(1f);
        StickerDoneManager.Instance.AddStickerDone(this);
        //GetPosMoveTo();
        //return UniTask.CompletedTask;
    }

    public UniTask OnUpdateOpenState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitOpenState()
    {
        return UniTask.CompletedTask;
    }

    private StickerPos stickerPos;

    public void GetPosMoveTo()
    {
        stickerPos = Level.Instance.oSController.GetFolderPos(stickerId);
        if (stickerPos != null)
        {
            stickerPos.RegisterObj(this);
            stateMachine.RequestTransition(StickerDoneMoveToObjHaveStickerState);
            return;
        }

        stickerPos = Level.Instance.fSpaceController.GetFreeSpacePos();
        if (stickerPos != null)
        {
            stickerPos.RegisterObj(this);
            stateMachine.RequestTransition(StickerDoneMoveFreeSpaceState);
            return;
        }

        stateMachine.RequestTransition(StickerDoneMoveAround);
    }
}