using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

public class SlotFolderLockByAdsState : IState
{
    public interface IHandler
    {
        UniTask OnEnterLockByAdsState();
        UniTask OnUpdateLockByAdsState();
        UniTask OnExitLockByAdsState();
    }
    private IHandler handler;
    
    public SlotFolderLockByAdsState(IHandler owner)
    {
        handler = owner;
    }
    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterLockByAdsState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateLockByAdsState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitLockByAdsState();
    }
}
public partial class SlotFolder : SlotFolderLockByAdsState.IHandler
{
    private SlotFolderLockByAdsState SlotFolderLockByAdsStateCache { get; set; }
    public SlotFolderLockByAdsState SlotFolderLockByAdsState => SlotFolderLockByAdsStateCache ??= new SlotFolderLockByAdsState(this);

    public UniTask OnEnterLockByAdsState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateLockByAdsState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitLockByAdsState()
    {
        return UniTask.CompletedTask;
    }
}