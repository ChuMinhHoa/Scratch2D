using System.Threading;
using BaseGame.Scripts.Unit.Card.State;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

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
    public async UniTask OnEnterDoneState()
    {
        await AnimCardDone();
    }

    public UniTask OnUpdateDoneState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitDoneState()
    {
        return UniTask.CompletedTask;
    }
    
    private async UniTask AnimCardDone()
    {
        await UniTask.WaitForSeconds(1f);
        await cardGraphic.AnimCardDone(() =>
        {
            scratchCardManager.EnableInput(false);
        });
        ResetCard();
        GamePlayManager.Instance.NextLayer();
        GamePlayManager.Instance.level.layerController.RemoveCard(this);
    }
}