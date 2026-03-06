using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;

public class StickerDoneInitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterDoneInitState();
        UniTask OnUpdateDoneInitState();
        UniTask OnExitDoneInitState();
    }
    private IHandler handler;
    
    public StickerDoneInitState(IHandler owner)
    {
        handler = owner;
    }
    
    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterDoneInitState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateDoneInitState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitDoneInitState();
    }
}

public partial class StickerDone : StickerDoneInitState.IHandler
{
    private StickerDoneInitState StickerDoneInitStateCache { get; set; }
    public StickerDoneInitState StickerDoneInitState => StickerDoneInitStateCache ??= new StickerDoneInitState(this);
    
    public void InitStickerMove(int id)
    {
        stickerId = id;
        var sprite = SpriteGlobalConfig.Instance.GetStickerIcon(id);
        sprIcon.sprite = sprite;
        stickerGlow.sprite = sprite;
        
        stateMachine.RequestTransition(StickerDoneOpenState);
    }
    
    public UniTask OnEnterDoneInitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateDoneInitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitDoneInitState()
    {
        return UniTask.CompletedTask;
    }
}