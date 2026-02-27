using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

public class CardInitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterInitState();
        UniTask OnUpdateInitState();
        UniTask OnExitInitState();
    }

    private IHandler handler;

    public CardInitState(IHandler owner)
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
public partial class Card : CardInitState.IHandler
{
    private CardInitState CardInitStateCache { get; set; }
    public CardInitState CardInitState => CardInitStateCache ??= new CardInitState(this);
    public UniTask OnEnterInitState()
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnUpdateInitState()
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnExitInitState()
    {
        throw new System.NotImplementedException();
    }
}