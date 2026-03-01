using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class FHSInitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterInitState();
        UniTask OnUpdateInitState();
        UniTask OnExitInitState();
    }
    
    public FHSInitState(IHandler owner)
    {
        handler = owner;
    }
    
    private IHandler handler;

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

public partial class FolderHaveSticker : FHSInitState.IHandler
{
    private FHSInitState FhsInitStateCache { get; set; }
    public FHSInitState FhsInitState => FhsInitStateCache ??= new FHSInitState(this);
    public ObjHaveStickerData data;
    public UniTask OnEnterInitState()
    {
        InitData();
        stateMachine.RegisterState(FhsWaitState);
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateInitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitInitState()
    {
        return UniTask.CompletedTask;
    }

    private void InitData()
    {
        for (var i = 0; i < data.stickerIds.Length; i++)
        {
            trsStickerPos[i].id = data.stickerIds[i];
            var spriteIcon = SpriteGlobalConfig.Instance.GetStickerIcon(trsStickerPos[i].id);
            if (spriteIcon)
            {
                sprStickerIcons[i].sprite = spriteIcon;
            }
        }
    }

    public void LoadData(ObjHaveStickerData dataChange)
    {
        data = dataChange;
        stateMachine.RequestTransition(FhsInitState);
    }
}
