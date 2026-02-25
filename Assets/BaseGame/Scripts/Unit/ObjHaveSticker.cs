using System;
using UnityEngine;

public class ObjHaveSticker : MonoBehaviour
{
    public StickerPos[] trsStickerPos;

    public bool IsSameSticker(int id, out StickerPos stickerPos)
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (trsStickerPos[i].id != id) continue;

            if (trsStickerPos[i].IsHaveSticker()) continue;
            
            stickerPos = trsStickerPos[i];
            return true;
        }
        stickerPos = null;
        return false;
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