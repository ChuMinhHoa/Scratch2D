using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class SlotFolderInitState : IState
{
    public interface IHandler
    {
        UniTask OnFolderInitEnter();
        UniTask OnFolderInitUpdate();
        UniTask OnFolderInitExit();
    }

    private IHandler handler;

    public SlotFolderInitState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnFolderInitEnter();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnFolderInitUpdate();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnFolderInitExit();
    }
}

public partial class SlotFolder : SlotFolderInitState.IHandler
{
    private SlotFolderInitState SlotFolderInitStateCache { get; set; }
    public SlotFolderInitState SlotFolderInitState => SlotFolderInitStateCache ??= new SlotFolderInitState(this);

    public UniTask OnFolderInitEnter()
    {
        slotFolderGraphic.ChangeType(slotFolderType);
        btnWatchAds.SetActive(slotFolderType == SlotFolderType.Ads);
        btnBuy.SetActive(slotFolderType == SlotFolderType.Coin);
        return UniTask.CompletedTask;
    }

    public UniTask OnFolderInitUpdate()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnFolderInitExit()
    {
        return UniTask.CompletedTask;
    }
}