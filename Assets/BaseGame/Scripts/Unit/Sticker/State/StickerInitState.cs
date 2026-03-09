
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

    public Transform currentParents;
    public async UniTask OnEnterInitState()
    {
        await stickerGraphic.InitStickerType(stickerData.stickerType);
        stickerGraphic.SetParents(currentParents);
    }

    public UniTask OnUpdateInitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitInitState()
    {
        return UniTask.CompletedTask;
    }

    public void SetParents(Transform stickerPoint)
    {
        currentParents = stickerPoint;
    }
}