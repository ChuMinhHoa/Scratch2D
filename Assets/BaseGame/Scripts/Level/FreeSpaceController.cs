using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public SpaceSticker[] spaceStickers;
    
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
    }
}