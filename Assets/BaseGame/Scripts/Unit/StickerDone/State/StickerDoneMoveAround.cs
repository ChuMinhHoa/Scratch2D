using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class StickerDoneMoveAround : IState
{
    public interface IHandler
    {
        UniTask OnEnterMoveAroundState();
        UniTask OnUpdateMoveAroundState();
        UniTask OnExitMoveAroundState();
    }
    private IHandler handler;

    public StickerDoneMoveAround(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterMoveAroundState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateMoveAroundState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitMoveAroundState();
    }
}

public partial class StickerDone : StickerDoneMoveAround.IHandler
{
    private StickerDoneMoveAround StickerDoneMoveAroundCache { get; set; }
    public StickerDoneMoveAround StickerDoneMoveAround => StickerDoneMoveAroundCache ??= new StickerDoneMoveAround(this);
    
    private Vector3 centerPosition;
    private float orbitRadius = 0.3f;
    private MotionHandle orbitMotionHandle;
    
    public UniTask OnEnterMoveAroundState()
    {
        Level.Instance.fSpaceController.RegisterStickerDoneWait(this);
        centerPosition = transform.position;
        orbitMotionHandle = LMotion.Create(0f, Mathf.PI * 2f, 1f)
            .WithLoops(-1)
            .WithEase(Ease.Linear)
            .Bind(angle =>
            {
                var x = centerPosition.x + Mathf.Cos(angle) * orbitRadius;
                var y = centerPosition.y + Mathf.Sin(angle) * orbitRadius;
                transform.position = new Vector3(x, y, centerPosition.z);
            });
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateMoveAroundState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitMoveAroundState()
    {
        if (orbitMotionHandle.IsActive())
            orbitMotionHandle.Cancel();
        transform.position = centerPosition;
        return UniTask.CompletedTask;
    }
}