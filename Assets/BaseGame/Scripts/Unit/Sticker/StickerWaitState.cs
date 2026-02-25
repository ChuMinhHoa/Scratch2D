using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerWaitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterWaitState();
        UniTask OnUpdateWaitState();
        UniTask OnExitWaitState();
    }
    
    private IHandler handler;

    public StickerWaitState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterWaitState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateWaitState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitWaitState();
    }
}

public partial class Sticker : StickerWaitState.IHandler
{
    private StickerWaitState StickerWaitStateCache { get; set; }
    public StickerWaitState StickerWaitState => StickerWaitStateCache ??= new StickerWaitState(this);
    public UniTask OnEnterWaitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateWaitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitWaitState()
    {
        return UniTask.CompletedTask;
    }
}
