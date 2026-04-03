using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class CardFreezeState : IState
{
    public interface IHandler
    {
        UniTask OnEnterFreezeState();
        UniTask OnUpdateFreezeState();
        UniTask OnExitFreezeState();
    }

    private IHandler handler;

    public CardFreezeState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterFreezeState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateFreezeState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitFreezeState();
    }
}

public partial class Card : CardFreezeState.IHandler
{
    private CardFreezeState CardFreezeStateCache { get; set; }
    public CardFreezeState CardFreezeState => CardFreezeStateCache ??= new CardFreezeState(this);

    private int countForFreeze = 0;

    public UniTask OnEnterFreezeState()
    {
        countForFreeze = 0;
        GlobalEventManager.OnNoteDoneCallBack += OnNoteDoneForFreeze;
        return UniTask.CompletedTask;
    }

    private void OnNoteDoneForFreeze()
    {
        if (!isShowed)
            return;
        countForFreeze++;
        if (countForFreeze == 3)
        {
            cardGraphic.OnFreezeDone();
            GlobalEventManager.OnNoteDoneCallBack -= OnNoteDoneForFreeze;
            ChangeCardState(CardState.Normal);
            _ = WaitForEnableInput();
            SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_FreezeEnd);
        }
        else
        {
            cardGraphic.SetSpriteFreeze(countForFreeze);
            SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_FreezeBreak);
        }
    }

    private async UniTask WaitForEnableInput()
    {
        await UniTask.WaitUntil(() => stateMachine.CurrentState == CardWaitState);
        OnShowMode();
    }

    public UniTask OnUpdateFreezeState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitFreezeState()
    {
        return UniTask.CompletedTask;
    }
}