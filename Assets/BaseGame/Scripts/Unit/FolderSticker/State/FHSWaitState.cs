using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class FHSWaitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterWaitState();
        UniTask OnUpdateWaitState();
        UniTask OnExitWaitState();
    }
    
    public FHSWaitState(IHandler owner)
    {
        handler = owner;
    }
    
    private IHandler handler;

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

public partial class FolderHaveSticker : FHSWaitState.IHandler
{
    private FHSWaitState fhsWaitStateCache { get; set; }
    public FHSWaitState FhsWaitState => fhsWaitStateCache ??= new FHSWaitState(this);

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
