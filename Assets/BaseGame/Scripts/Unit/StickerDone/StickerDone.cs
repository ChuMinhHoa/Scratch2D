using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public partial class StickerDone : MonoBehaviour
{
    public UnitAnimation unitAnim;
    public UnitAnimation unitAnimMoveToCart;
    public UnitAnimation unitAnimMoveToFreeSpaceOnBooster;
    public SpriteRenderer sprIcon;
    public Animation stickerDoneAnim;
    public SpriteRenderer stickerGlow;
    public StateMachine stateMachine = new();
    public int stickerId;

    private MotionHandle motionMoveHandle;
    
    private CancellationToken destroyToken;

    private void Start()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
        stateMachine.Run();
        stateMachine.RequestTransition(StickerDoneInitState);
    }

    private void OnDestroy()
    {
        //destroyToken.
    }

    private void CheckToAbleStickerAnimAgain()
    {
        if (stickerDoneAnim.enabled == false)
        {
            stickerDoneAnim.enabled = true;
            stickerDoneAnim.Play("Idle");
        }
    }

    public void ResetStickerDone()
    {
        stickerGlow?.gameObject.SetActive(false);
    }

    public void CheckMoveToFolder(bool fromNoWhere = false, bool fromFreeSpace = false)
    {
        var e = Level.Instance.oSController.GetFolderPos(this);
        if (e != null)
        {
            if (e.TryReserve(this))
            {
                Level.Instance.fSpaceController.RemoveStickerDoneFromNoWhere(this);
                Level.Instance.RemoveStickerDone(this);
                Level.Instance.fSpaceController.RemoveStickerDoneFromCart(this);
                if (stickerPos != null)
                {
                    Level.Instance.fSpaceController.ResetPos(stickerPos);
                }
                //stickerPos?.ResetPos();
                stickerPos = e;
                stateMachine.RequestTransition(StickerDoneMoveToObjHaveStickerState);
                return;
            }
        }

        if (!fromFreeSpace && stateMachine.CurrentState != StickerDoneWaitOnCartState)
        {
            e = Level.Instance.fSpaceController.GetFreeSpacePos(this);
            if (e != null)
            {
                if (e.TryReserve(this))
                {
                    stickerPos = e;
                    Level.Instance.fSpaceController.RemoveStickerDoneFromNoWhere(this);
                    Level.Instance.RemoveStickerDone(this);
                    stateMachine.RequestTransition(StickerDoneMoveFreeSpaceState);
                    return;
                }
            }
        }

        if (!fromNoWhere && !fromFreeSpace && stateMachine.CurrentState != StickerDoneWaitOnCartState)
            stateMachine.RequestTransition(StickerDoneMoveAround);
    }

    public bool IsHaveSticker(int noteId)
    {
        return noteId == stickerId;
    }
    
    public void SetActionCallBackOnMoveToNote(Action actionCallBack)
    {
        actionCallBackOnMoveToNote = actionCallBack;
    }

    private void ClearAnim()
    {
        unitAnim.ClearAnim();
        unitAnimMoveToFreeSpaceOnBooster.ClearAnim();
        unitAnimMoveToCart.ClearAnim();
    }
    public async UniTask MoveToPos(Vector3 target)
    {
        if (this == null || destroyToken.IsCancellationRequested)
            return;
        ClearAnim();
        await unitAnim.PlayMoveAnim(target).AttachExternalCancellation(destroyToken);
    }
    
    private async UniTask MoveToPosLocal(Vector3 target)
    {
        if (this == null || destroyToken.IsCancellationRequested)
            return;
        ClearAnim();
        await unitAnim.PlayMoveAnimLocal(target).AttachExternalCancellation(destroyToken);
    }
    
    private async UniTask MoveToCart(Vector3 target)
    {
        if (this == null || destroyToken.IsCancellationRequested)
            return;
        ClearAnim();
        await unitAnimMoveToCart.PlayMoveAnim(target).AttachExternalCancellation(destroyToken);
    }

    public void MoveToFreeSpaceOnUseBooster(Vector3 newPos)
    {
        if (this == null || destroyToken.IsCancellationRequested)
            return;
        ClearAnim();
        _ = unitAnimMoveToFreeSpaceOnBooster.PlayMoveAnim(newPos).AttachExternalCancellation(destroyToken);
    }
}