
using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerInitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterInitState();
        UniTask OnUpdateInitState();
        UniTask OnExitInitState();
    }
    
    private IHandler handler;

    public StickerInitState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterInitState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateInitState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitInitState();
    }
}

public partial class Sticker : StickerInitState.IHandler
{
    private StickerInitState StickerInitStateCache { get; set; }
    public StickerInitState StickerInitState => StickerInitStateCache ??= new StickerInitState(this);

    
    public UniTask OnEnterInitState()
    {
        stickerGraphic.InitData(stickerData.stickerID);
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateInitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitInitState()
    {
        return UniTask.CompletedTask;
    }
}