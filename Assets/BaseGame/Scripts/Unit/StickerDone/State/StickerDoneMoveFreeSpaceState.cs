using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneMoveFreeSpaceState : IState
{
    public interface IHandler
    {
        UniTask OnEnterMoveFreeSpace();
        UniTask OnUpdateMoveFreeSpace();
        UniTask OnExitMoveFreeSpace();
    }

    private IHandler handler;

    public StickerDoneMoveFreeSpaceState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterMoveFreeSpace();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateMoveFreeSpace();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitMoveFreeSpace();
    }
}

public partial class StickerDone : StickerDoneMoveFreeSpaceState.IHandler
{
    private StickerDoneMoveFreeSpaceState StickerDoneMoveFreeSpaceStateCache { get; set; }
    public StickerDoneMoveFreeSpaceState StickerDoneMoveFreeSpaceState => StickerDoneMoveFreeSpaceStateCache ??= new StickerDoneMoveFreeSpaceState(this);
    
    public async UniTask OnEnterMoveFreeSpace()
    {
        actionCallBackOnMoveToNote?.Invoke(); 
        UnitEventManager.Instance.RegisterEvent(gameObject);
        stickerPos.RegisterObj(this);
        stickerDoneAnim.enabled = false;
        var currentScale = sprIcon.transform.localScale;
        var targetScale = stickerPos.trsPos.localScale;
        var currentEulerAngle = transform.eulerAngles;
        
        LMotion.Create(currentScale, targetScale, .25f).Bind(x => sprIcon.transform.localScale = x).AddTo(this);
        
        LMotion.Create(currentEulerAngle, stickerPos.trsPos.eulerAngles, .25f).Bind(x => transform.eulerAngles = x).AddTo(this);
        
        await unitAnim.PlayMoveAnim(stickerPos.trsPos.position);
        SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_StickerDoneFSpace);
        CheckToAbleStickerAnimAgain();
        stickerPos.MoveDone();
        Level.Instance.CheckStickerDone();
        UnitEventManager.Instance.RemoveEventId(gameObject);

        if (UnitEventManager.Instance.IsHaveEvent())
        {
            return;
        }
        
        Level.Instance.CheckLoseGame();
    }

    public UniTask OnUpdateMoveFreeSpace()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitMoveFreeSpace()
    {
        return UniTask.CompletedTask;
    }
}