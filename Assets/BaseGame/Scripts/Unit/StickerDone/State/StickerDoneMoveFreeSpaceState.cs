using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneMoveFreeSpaceState : IState
{
    public interface IHandler
    {
        UniTask OnEnterMoveFreeSpace();
        UniTask OnUpdateMoveFreeSpace();
        UniTask OnExitMoveFreeSpace();
    }

    private IHandler handler;

    public StickerDoneMoveFreeSpaceState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterMoveFreeSpace();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateMoveFreeSpace();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitMoveFreeSpace();
    }
}

public partial class StickerDone : StickerDoneMoveFreeSpaceState.IHandler
{
    private StickerDoneMoveFreeSpaceState StickerDoneMoveFreeSpaceStateCache { get; set; }
    public StickerDoneMoveFreeSpaceState StickerDoneMoveFreeSpaceState => StickerDoneMoveFreeSpaceStateCache ??= new StickerDoneMoveFreeSpaceState(this);
    public async UniTask OnEnterMoveFreeSpace()
    {
        stickerPos.RegisterObj(this);
        stickerDoneAnim.enabled = false;
        //LMotion.Create(sprIcon.transform.localPosition, Vector3.zero, .25f).Bind(x => sprIcon.transform.localPosition = x);
        await unitAnim.PlayMoveAnim(stickerPos.trsPos.position);
        stickerPos.MoveDone();
        Level.Instance.CheckStickerDone();
        //return UniTask.CompletedTask;
    }

    public UniTask OnUpdateMoveFreeSpace()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitMoveFreeSpace()
    {
        return UniTask.CompletedTask;
    }
}