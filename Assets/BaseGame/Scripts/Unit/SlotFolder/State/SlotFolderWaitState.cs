using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

public class SlotFolderWaitState : IState
{
    public interface IHandler
    {
        UniTask OnWaitEnterState();
        UniTask OnWaitUpdateState();
        UniTask OnWaitExitState();
    }
    
    private IHandler handler;
    
    public SlotFolderWaitState(IHandler owner)
    {
        handler = owner;
    }
    
    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnWaitEnterState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnWaitUpdateState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnWaitExitState();
    }
}

public partial class SlotFolder : SlotFolderWaitState.IHandler
{
    private SlotFolderWaitState SlotFolderWaitStateCache { get; set; }
    public SlotFolderWaitState SlotFolderWaitState => SlotFolderWaitStateCache ??= new SlotFolderWaitState(this);
    
    public UniTask OnWaitEnterState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnWaitUpdateState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnWaitExitState()
    {
        return UniTask.CompletedTask;
    }
}