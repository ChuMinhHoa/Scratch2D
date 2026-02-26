using System;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public SpaceSticker[] spaceStickers;

    public void SetOSController(Reactive<ObjHaveSticker> oSc) => currentObjHaveSticker = oSc;

    public override bool RegisterSticker(Sticker sticker)
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].IsFreeSpace(out var stickerPos)) continue;
            _ = SpawnStickerDone(stickerPos, sticker, true);
            return true;
        }

        return false;
    }
    
    public void CheckAllStickerOnFreeSpace()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveSticker())
                continue;
            if (RegisterStickerDoneToObject(spaceStickers[i].stickerPos.stickerDone))
            {
                spaceStickers[i].stickerPos.ResetStickerPos();
            }
        }
    }
}