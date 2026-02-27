using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

public class ObjHaveSticker : MonoBehaviour
{
    public int objId;
    public List<StickerPos> trsStickerPos;
    public SpriteRenderer sprIcon;
    public Transform posParents;
    public List<SpriteRenderer> sprStickerIcon;
    
    public UnitAnimation unitAnim;
    public AnimationCurve curveScaleToCurrentObj;
    
    public bool IsSameSticker(int id, out StickerPos stickerPos)
    {
        for (var i = 0; i < trsStickerPos.Count; i++)
        {
            if (trsStickerPos[i].id != id) continue;

            if (trsStickerPos[i].IsHaveSticker()) continue;
            
            stickerPos = trsStickerPos[i];
            return true;
        }
        stickerPos = null;
        return false;
    }

    public void InitData(ObjHaveStickerData data)
    {
        sprIcon.sprite = SpriteGlobalConfig.Instance.GetIconObjectHaveSticker(data.objID);
        for (var i = 0; i < data.stickerIds.Length; i++)
        {
            var stickerPos = new StickerPos();
            trsStickerPos.Add(stickerPos);
            stickerPos.id = data.stickerIds[i];
            var spriteIcon = SpriteGlobalConfig.Instance.GetStickerIcon(stickerPos.id);
            var e = PoolManager.Instance.SpawnPosSticker(posParents);
            stickerPos.trsStickerPos = e.transform;
            e.sprite = spriteIcon;
            e.transform.localPosition = data.stickerPositions[i];
            e.transform.localScale = data.stickerScales[i];
            e.transform.eulerAngles = data.stickerRotations[i];
            sprStickerIcon.Add(e);
        }
    }

    private void ResetObjHaveSticker()
    {
        for (var i = 0; i < sprStickerIcon.Count; i++)
        {
            PoolManager.Instance.DeSpawnPosSticker(sprStickerIcon[i]);
        }

        for (var i = 0; i < trsStickerPos.Count; i++)
        {
            if (!trsStickerPos[i].stickerDone)continue;
            trsStickerPos[i].stickerDone.ResetStickerDone();
            PoolManager.Instance.DespawnStickerMove(trsStickerPos[i].stickerDone);
        }
        
        trsStickerPos.Clear();
        sprStickerIcon.Clear();
        PoolManager.Instance.DespawnObjHaveSticker(this);
    }

    public bool IsCompleteSticker()
    {
        for(var i = 0; i < trsStickerPos.Count; i++)
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
        await LMotion.Create(currentPos, posOut.position, 0.25f).Bind(x => transform.position = x).AddTo(this);
        ResetObjHaveSticker();
    }

    public void MoveToTarget(Vector3 target, bool isCurrent)
    {
        _ = unitAnim.PlayMoveAnim(target);
        if (isCurrent)
        {
            var currentScale = transform.localScale;
            LMotion.Create(currentScale, Vector3.one, 0.25f).WithEase(curveScaleToCurrentObj).Bind(x => transform.localScale = x).AddTo(this);
        }
    }

    public void AnimFirstSpawn(int indexObjSpawn)
    {
    }
}

[Serializable]
public class StickerPos
{
    public int id;
    public Transform trsStickerPos;
    public StickerDone stickerDone;
    public bool moveDone;
    
    public void RegisterSticker(StickerDone stickerRegister)
    {
        stickerDone = stickerRegister;
    }

    public bool IsHaveSticker()
    {
        return stickerDone != null;
    }

    public void MoveDone()
    {
        moveDone = true;
    }

    public bool IsMoveDone() => moveDone;
    public void ResetStickerPos()
    {
        stickerDone = null;
        moveDone = false;
    }
}