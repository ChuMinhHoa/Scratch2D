using System.Threading;
using BaseGame.Scripts.Unit.Card.State;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

namespace BaseGame.Scripts.Unit.Card.State
{
    public class CardDoneState : IState
    {
        public interface IHandler
        {
            UniTask OnEnterDoneState();
            UniTask OnUpdateDoneState();
            UniTask OnExitDoneState();
        }

        private IHandler handler;

        public CardDoneState(IHandler owner)
        {
            handler = owner;
        }

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
}

public partial class Card : CardDoneState.IHandler
{
    private CardDoneState CardDoneStateCache { get; set; }
    public CardDoneState CardDoneState => CardDoneStateCache ??= new CardDoneState(this);
    public UniTask OnEnterDoneState()
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnUpdateDoneState()
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnExitDoneState()
    {
        throw new System.NotImplementedException();
    }
}