using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ObjHaveStickerController : SpaceForSticker
{
    public List<ObjHaveSticker> objHaveStickers = new();
    private int indexCurrentObjHaveSticker;
    public Transform posParents;
    public Transform posFirstSpawn;
    public Transform posOut;
    public Vector3 offSet;
    public Vector3 scaleOnWaiting;
    public bool loadDone;
    
    public void LoadData(Span<ObjHaveStickerData> objSticker)
    {
        for (var i = 0; i < objSticker.Length; i++)
        {
            var ot = PoolManager.Instance.SpawnObjHaveSticker();
            ot.InitData(objSticker[i]);
            ot.transform.localScale = scaleOnWaiting;
            // ot.transform.position = posParents.position + offSet * i;
            ot.transform.position = posFirstSpawn.position;
            ot.AnimFirstSpawn(i);
            objHaveStickers.Add(ot);
        }

        loadDone = true;
    }

    public override bool RegisterSticker(Sticker sticker)
    {
        if (currentObjHaveSticker.Value == null) return false;
        if (!currentObjHaveSticker.Value.IsSameSticker(sticker.stickerData.stickerID, out var stickerPos)) return false;
        _ = SpawnStickerDone(stickerPos, sticker);
        return true;
    }

    public void SetCurrentObjHaveSticker(ObjHaveSticker objHaveSticker)
    {
        currentObjHaveSticker.Value = objHaveSticker;
    }
 
    public async UniTask CallNextObjSticker()
    {
        await UniTask.WaitForSeconds(0.5f);
        if (currentObjHaveSticker.Value)
            await currentObjHaveSticker.Value.MoveOut(posOut);
        if (indexCurrentObjHaveSticker < objHaveStickers.Count)
        {
            for (var i = 0; i < objHaveStickers.Count; i++)
            {
                if (i < indexCurrentObjHaveSticker) continue;
                var target = posParents.position + offSet * (i - indexCurrentObjHaveSticker);
                var isCurrent = i == indexCurrentObjHaveSticker;
                objHaveStickers[i].MoveToTarget(target, isCurrent);
            }
            SetCurrentObjHaveSticker(objHaveStickers[indexCurrentObjHaveSticker]);
            indexCurrentObjHaveSticker++;
        }
        else
        {
            Debug.Log("End game!");
            ResetController();
        }
    }

    public override void ResetController()
    {
        currentObjHaveSticker.Value = null;
        objHaveStickers.Clear();
        indexCurrentObjHaveSticker = 0;
    }
}