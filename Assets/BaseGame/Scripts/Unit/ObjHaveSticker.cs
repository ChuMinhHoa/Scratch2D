using System;
using UnityEngine;

public class ObjHaveSticker : MonoBehaviour
{
    public StickerPos[] trsStickerPos;

    public bool IsSameSticker(int id, out Transform stickerPos)
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (trsStickerPos[i].id != id) continue;

            if (trsStickerPos[i].IsHaveSticker()) continue;
            
            stickerPos = trsStickerPos[i].trsStickerPos;
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
    public Sticker sticker;
    
    public void RegisterSticker(Sticker stickerRegister)
    {
        sticker = stickerRegister;
    }

    public bool IsHaveSticker()
    {
        return sticker != null;
    }
}