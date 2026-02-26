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
        Debug.Log("Load ObjHaveSticker Data");
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

    public void ResetObjHaveSticker()
    {
        for (var i = 0; i < sprStickerIcon.Count; i++)
        {
            PoolManager.Instance.DeSpawnPosSticker(sprStickerIcon[i]);
        }
        
        trsStickerPos.Clear();
        PoolManager.Instance.DespawnObjHaveSticker(this);
    }

    public bool IsCompleteSticker()
    {
        for(var i = 0; i < trsStickerPos.Count; i++)
        {
            if (!trsStickerPos[i].IsHaveSticker())
                return false;
        }
        return true;
    }

    public async UniTask Move(Transform posOut)
    {
        var currentPos = transform.position;
        await LMotion.Create(currentPos, posOut.position, 0.25f).Bind(x => transform.position = x).AddTo(this);
        ResetObjHaveSticker();
    }
}

[Serializable]
public class StickerPos
{
    public int id;
    public Transform trsStickerPos;
    public StickerDone sticker;
    
    public void RegisterSticker(StickerDone stickerRegister)
    {
        sticker = stickerRegister;
    }

    public bool IsHaveSticker()
    {
        return sticker != null;
    }
}