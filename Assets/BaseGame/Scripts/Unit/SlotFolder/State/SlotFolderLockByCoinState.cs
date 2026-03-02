using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

public class SlotFolderLockByCoinState : IState
{
    public interface IHandler
    {
        UniTask OnEnterLockByCoinState();
        UniTask OnUpdateLockByCoinState();
        UniTask OnExitLockByCoinState();
    }

    private IHandler handler;

    public SlotFolderLockByCoinState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterLockByCoinState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateLockByCoinState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitLockByCoinState();
    }
}
public partial class SlotFolder : SlotFolderLockByCoinState.IHandler
{
    private SlotFolderLockByCoinState SlotFolderLockByCoinStateCache { get; set; }
    public SlotFolderLockByCoinState SlotFolderLockByCoinState => SlotFolderLockByCoinStateCache ??= new SlotFolderLockByCoinState(this);

    public UniTask OnEnterLockByCoinState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateLockByCoinState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitLockByCoinState()
    {
        return UniTask.CompletedTask;
    }
}