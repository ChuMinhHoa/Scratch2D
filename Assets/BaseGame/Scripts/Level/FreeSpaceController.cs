using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public SpaceSticker[] spaceStickers;
    [ShowInInspector] public List<StickerDone> stickerCantMoveAnyWhere = new();
    
    public override bool RegisterSticker(Sticker sticker)
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].IsFreeSpace(out var stickerPos)) continue;
            _ = SpawnStickerDone(stickerPos, sticker, null);
            return true;
        }

        return false;
    }
    
    public void CheckAllStickerOnFreeSpace()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj())
                continue;
            if(GamePlayManager.Instance.level.oSController.RegisterStickerDoneFromFreeSpace(spaceStickers[i].stickerPos.obj))
            {
                spaceStickers[i].stickerPos.ResetPos();
            }
        }

        for (var i = stickerCantMoveAnyWhere.Count - 1; i >= 0; i--) 
        {
            if(GamePlayManager.Instance.level.oSController.RegisterStickerDoneFromFreeSpace(stickerCantMoveAnyWhere[i]))
            {
                stickerCantMoveAnyWhere.Remove(stickerCantMoveAnyWhere[i]);
            }
        }

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj())
                return;
        }
        if (stickerCantMoveAnyWhere.Count == 0)
            return;
        GamePlayManager.Instance.level.GameOver();
    }

    public override async UniTask SpawnStickerDoneNotMove(Sticker sticker)
    {
        await base.SpawnStickerDoneNotMove(sticker);
        var position = sticker.transform.position;
        position.z = -1;
        var e = PoolManager.Instance.SpawnStickerDone(sticker.transform);
        
        stickerCantMoveAnyWhere.Add(e);
        
        e.transform.eulerAngles = Vector3.zero;
        e.InitStickerMove(sticker.stickerData.stickerID);
        e.transform.position = position;
        sticker.DisAbleIcon();
        e.PlayAnimRemove();
        await UniTask.WaitForSeconds(3f);
        CheckGameOver();
    }
}