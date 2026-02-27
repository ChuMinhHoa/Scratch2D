using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

public class CardWaitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterWaitState();
        UniTask OnUpdateWaitState();
        UniTask OnExitWaitState();
    }

    private IHandler handler;

    public CardWaitState(IHandler owner)
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

public partial class Card : CardWaitState.IHandler
{
    private CardWaitState CardWaitStateCache { get; set; }
    public CardWaitState CardWaitState => CardWaitStateCache ??= new CardWaitState(this);

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