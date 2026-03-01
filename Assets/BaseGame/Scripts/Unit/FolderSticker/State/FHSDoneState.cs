using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class FHSDoneState  : IState
{
    public interface IHandler
    {
        UniTask OnEnterDoneState();
        UniTask OnUpdateDoneState();
        UniTask OnExitDoneState();
    }
    
    public FHSDoneState(IHandler owner)
    {
        handler = owner;
    }
    
    private IHandler handler;

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

public partial class FolderHaveSticker : FHSDoneState.IHandler
{
    private FHSDoneState fhsDoneStateCache { get; set; }
    public FHSDoneState FhsDoneState => fhsDoneStateCache ??= new FHSDoneState(this);

    public UniTask OnEnterDoneState()
    {
        return UniTask.CompletedTask;
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
