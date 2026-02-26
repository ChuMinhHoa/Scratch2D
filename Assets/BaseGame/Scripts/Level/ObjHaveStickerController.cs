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
    
    public void LoadData(Span<ObjHaveStickerData> objSticker)
    {
        for (var i = 0; i < objSticker.Length; i++)
        {
            var ot = PoolManager.Instance.SpawnObjHaveSticker();
            ot.InitData(objSticker[i]);
            // ot.transform.position = posParents.position + offSet * i;
            ot.transform.position = posFirstSpawn.position;
            objHaveStickers.Add(ot);
        }
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
        _ = currentObjHaveSticker.Value?.Move(posOut);
        if (indexCurrentObjHaveSticker < objHaveStickers.Count)
        {
            for (var i = 0; i < objHaveStickers.Count; i++)
            {
                if (i < indexCurrentObjHaveSticker) continue;
                var currentPos = objHaveStickers[i].transform.position;
                var target = posParents.position + offSet * (i - indexCurrentObjHaveSticker);
                var index = i;
                LMotion.Create(currentPos, target, 0.25f).WithDelay(0.15f * i).Bind(x => objHaveStickers[index].transform.position = x)
                    .AddTo(objHaveStickers[i]);
            }
            SetCurrentObjHaveSticker(objHaveStickers[indexCurrentObjHaveSticker]);
            indexCurrentObjHaveSticker++;
        }
        else
        {
            Debug.Log("End game!");
        }
    }

    public override void ResetController()
    {
        indexCurrentObjHaveSticker = 0;
    }
}