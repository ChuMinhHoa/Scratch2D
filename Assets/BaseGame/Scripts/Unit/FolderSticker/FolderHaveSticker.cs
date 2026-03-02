using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;
using UnityEngine.Serialization;

public partial class FolderHaveSticker : MonoBehaviour
{
    public int objId;
    public StickerPos[] trsStickerPos;
    public SpriteRenderer sprIcon;
    public SpriteRenderer[] sprStickerIcons;
    
    public UnitAnimation unitAnim;
    public FHSGraphic fhsGraphic;
    public StateMachine stateMachine;
    public bool readyToMove;
    private void Start()
    {
        stateMachine.RequestTransition(FhsWaitState);
        stateMachine.Run();
    }

    public bool IsSameSticker(int id, out StickerPos stickerPos)
    {
        // if (!readyToMove)
        // {
        //     stickerPos = null;
        //     return false;
        // }
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (trsStickerPos[i].id != id) continue;

            if (trsStickerPos[i].IsHaveObj()) continue;
            
            stickerPos = trsStickerPos[i];
            return true;
        }
        stickerPos = null;
        return false;
    }

    private void ResetFolderSticker()
    {
        readyToMove = false;
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (!trsStickerPos[i].obj)continue;
            trsStickerPos[i].obj.ResetStickerDone();
            PoolManager.Instance.DespawnStickerMove(trsStickerPos[i].obj);
            trsStickerPos[i].ResetPos();
        }
        PoolManager.Instance.DespawnObjHaveSticker(this);
    }

    public bool IsCompleteSticker()
    {
        for(var i = 0; i < trsStickerPos.Length; i++)
        {
            if (!trsStickerPos[i].IsMoveDone())
                return false;
        }
        return true;
    }

    public async UniTask MoveOut(Transform posOut)
    {
        var currentPos = transform.position;
        await unitAnim.PlayScaleAnimation();
        await fhsGraphic.OnClose();
        await LMotion.Create(currentPos, posOut.position, 0.25f).Bind(x => transform.position = x).AddTo(this);
        ResetFolderSticker();
    }

    public async UniTask MoveToTarget(Vector3 target)
    {
        await unitAnim.PlayMoveAnim(target);
        await fhsGraphic.OnOpen();
        readyToMove = true;
        GamePlayManager.Instance.level.CheckAllStickerOnFreeSpace();
    }
}

[Serializable]
public class StickerPos : ObjPos<StickerDone>
{
    public int id;
    public bool moveDone;

    public void MoveDone()
    {
        moveDone = true;
    }

    public bool IsMoveDone() => moveDone;
    public override void ResetPos()
    {
        id = -1;
        obj = null;
        moveDone = false;
    }
}

[Serializable]
public class FolderPos : ObjPos<FolderHaveSticker>
{
    public int id;
    public bool moveDone;

    public void MoveDone()
    {
        moveDone = true;
    }

    public bool IsMoveDone() => moveDone;
    public override void ResetPos()
    {
        id = -1;
        obj = null;
        moveDone = false;
    }
}

[Serializable]
public class ObjPos<T>
{
    [FormerlySerializedAs("trsStickerPos")] public Transform trsPos;
    public T obj;
    
    public void RegisterObj(T objChange)
    {
        obj = objChange;
    }

    public bool IsHaveObj()
    {
        return obj != null;
    }
    public virtual void ResetPos()
    {
        obj = default;
    }
}
