using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public partial class FolderHaveSticker : MonoBehaviour
{
    public int objId;
    public StickerPos[] trsStickerPos;
    public UnitAnimation unitAnim;
    public FHSGraphic fhsGraphic;
    public StateMachine stateMachine;
    public bool readyToMove;
    private void Start()
    {
        stateMachine.RequestTransition(FhsWaitState);
        stateMachine.Run();

        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            trsStickerPos[i].moveDone.Skip(1).Subscribe(StickerMoveDone).AddTo(this);
        }
    }

    private void StickerMoveDone(bool stickerMoveDone)
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (!trsStickerPos[i].moveDone) return;
        }

        Level.Instance.MoveFolderOut(this);
    }

    public bool IsSameSticker(int id, out StickerPos stickerPos)
    {
        stickerPos = null;
        if (data.stickerId != id) return false;

        foreach (var pos in trsStickerPos)
        {
            if (pos.IsHaveObj()) continue;
            stickerPos = pos;
            return true;
        }
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

    public async UniTask MoveOut(Transform posOut)
    {
        var currentPos = transform.position;
        await unitAnim.PlayScaleAnimation();
        GlobalEventManager.CheckToCallNextSticker?.Invoke();
        await LMotion.Create(currentPos, posOut.position, 0.25f).Bind(x => transform.position = x).AddTo(this);
        ResetFolderSticker();
    }

    public async UniTask MoveToTarget(Vector3 target)
    {
        await unitAnim.PlayMoveAnim(target);
        readyToMove = true;
        await UniTask.WaitForSeconds(0.1f);
        Level.Instance.CheckStickerDone();
    }
}

[Serializable]
public class StickerPos : ObjPos<StickerDone>
{
    public int id;
    public Reactive<bool> moveDone;

    public void MoveDone()
    {
        moveDone.Value = true;
    }

    public bool IsMoveDone() => moveDone;
    public override void ResetPos()
    {
        id = -1;
        obj = null;
        moveDone.Value = false;
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
