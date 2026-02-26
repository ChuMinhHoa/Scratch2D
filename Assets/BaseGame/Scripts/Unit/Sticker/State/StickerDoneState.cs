using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneState : IState
{
    public interface IHandler
    {
        UniTask OnEnterDoneState();
        UniTask OnUpdateDoneState();
        UniTask OnExitDoneState();
    }

    private IHandler handler;

    public StickerDoneState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterDoneState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateDoneState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitDoneState();
    }
}

public partial class Sticker : StickerDoneState.IHandler
{
    private StickerDoneState StickerDoneStateCache { get; set; }
    public StickerDoneState StickerDoneState => StickerDoneStateCache ??= new StickerDoneState(this);

    public virtual async UniTask OnEnterDoneState()
    {
        await stickerGraphic.OnDoneMode();
        StickerMoveToTarget();
    }

    public UniTask OnUpdateDoneState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitDoneState()
    {
        return UniTask.CompletedTask;
    }
}