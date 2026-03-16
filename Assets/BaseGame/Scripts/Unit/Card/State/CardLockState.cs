using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class CardLockState : IState
{
    public interface IHandler
    {
        UniTask OnEnterLockState();
        UniTask OnUpdateLockState();
        UniTask OnExitLockState();
    } 
    
    private IHandler handler;

    public CardLockState(IHandler owner)
    {
        handler = owner;
    }
    
    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterLockState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateLockState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitLockState();
    }
}

public partial class Card : CardLockState.IHandler
{
    private CardLockState CardLockStateCache { get; set; }
    public CardLockState CardLockState => CardLockStateCache ??= new CardLockState(this);

    [ShowIf("@data.cardState == CardState.Lock")]
    private int countUnlockSticker = 0;

    public UniTask OnEnterLockState()
    {
        countUnlockSticker = data.totalSUnlock;
        cardGraphic.SetTextCount(countUnlockSticker);
        GlobalEventManager.OnNoteDoneCallBack += OnNoteDone;
        return UniTask.CompletedTask;
    }

    private void OnNoteDone()
    {
        countUnlockSticker--;
        cardGraphic.SetTextCount(countUnlockSticker);
        if (countUnlockSticker == 0)
        {
            GlobalEventManager.OnNoteDoneCallBack -= OnNoteDone;
            ChangeCardState(CardState.Normal);
            _ = WaitForCheckCard();
        }
    }

    private async UniTask WaitForCheckCard()
    {
        await UniTask.WaitUntil(() => stateMachine.CurrentState == CardWaitState);
        CheckToShow();
    }

    public UniTask OnUpdateLockState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitLockState()
    {
        return UniTask.CompletedTask;
    }
}
